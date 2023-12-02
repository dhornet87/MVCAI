
using DocumentDb;
using DocumentDb.Models;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Tesseract;

namespace MVCAI.Models
{
    public class DocumentModel(DocumentDbContext documentContext)
    {
        private DocumentDbContext _documentContext => documentContext;

        public async Task<List<ToDoViewModel>> GetTodos()
        {
            var list = await _documentContext.ToDos.OrderBy(x => x.DueDate).ThenBy(x => x.Done).ToListAsync();

            return list.Select(x => new ToDoViewModel { Titel = x.Title, Beschreibung = x.Details, Id = x.Id, DocId = x.DocId, Erledigt = x.Done, Faelligkeit = x.DueDate.Value.ToString("dd.MM.yyyy") }).ToList();
        }
        public async Task<List<DocumentViewModel>> GetDocuments()
        {
            var list = await _documentContext.Documents
                .Include(x => x.Maincategory)
                .Include(x => x.Subcategory)
                .Include(x => x.ToDos)
                .Include(x => x.Metadatas)
                .ToListAsync();

            var returnlist = new List<DocumentViewModel>();
            foreach (var item in list)
            {
                returnlist.Add(new DocumentViewModel
                {
                    Id = item.Id,
                    Hauptkategorie = item.Maincategory.Name,
                    Unterkategorie = item.Subcategory.Name,
                    Titel = item.Title,
                    Metadaten = item.Metadatas?.Select(x => new MetadataViewModel { Name = x.Description, Details = x.Details, Id = x.Id }).ToList() ?? new List<MetadataViewModel>(),
                    ToDos = item.ToDos?.Select(x =>
                    new ToDoViewModel { Id = x.Id, DocId = x.DocId, Beschreibung = x.Details, Titel = x.Title, Erledigt = x.Done, Faelligkeit = x.DueDate.Value.ToString("dd.MM.yyyy") }).ToList() ?? new List<ToDoViewModel>(),
                });
            }

            return returnlist;


        }
        public async Task AddToDo(Guid docId)
        {
            ToDo newTodo = new ToDo { DocId = docId };

            await _documentContext.ToDos.AddAsync(newTodo);

            await _documentContext.SaveChangesAsync();
        }
        public async Task AddMetaData(string name, Guid docId)
        {
            DocumentDb.Models.Metadata newMetadata = new DocumentDb.Models.Metadata { Description = name, DocId = docId };

            await _documentContext.Metadata.AddAsync(newMetadata);

            await _documentContext.SaveChangesAsync();

        }
        public DocumentViewModel ParseGPTText(string documentText)
        {
            var newDoc = new DocumentViewModel();
            var metadata = new List<MetadataViewModel>();
            //split text string into words/tokens
            var linearray = documentText.Split('\n');

            for (int i = 0; i < linearray.Length; i++)
            {
                if (linearray[i].StartsWith("Titel"))
                {

                }
                if (linearray[i].StartsWith("Hauptkategorie"))
                {
                    newDoc.Hauptkategorie = linearray[i].Split(':')[1];
                }
                if (linearray[i].StartsWith("Unterkategorie"))
                {
                    newDoc.Unterkategorie = linearray[i].Split(':')[1];
                }

                if (linearray[i].StartsWith('-'))
                {
                    metadata.Add(GetMetadata(linearray[i]));
                }
            }

            newDoc.Metadaten = metadata;


            return newDoc;
        }


        public async Task<DocumentViewModel> Save(DocumentViewModel documentVM)
        {
            var mainCatId = await SaveMaincategory(documentVM.Hauptkategorie);
            var subCatId = await SaveSubcategory(documentVM.Unterkategorie);

            var doc = await _documentContext.Documents.FindAsync(documentVM.Id);

            if (doc == null)
            {

                doc = new Document { MaincategoryId = mainCatId, SubcategoryId = subCatId, File = documentVM.File, Title = documentVM.Titel ?? "Platzhalter" };
                await _documentContext.AddAsync(doc);

            }
            else
            {
                doc.Title = documentVM.Titel;
                doc.MaincategoryId = mainCatId;
                doc.SubcategoryId = subCatId;
            }

            await _documentContext.SaveChangesAsync();

            documentVM.Id = doc.Id;

            documentVM.Metadaten = await SaveMetadata(documentVM.Metadaten, documentVM.Id);
            documentVM.ToDos = await SaveToDos(documentVM.ToDos, documentVM.Id);
            return documentVM;

        }
        public async Task SetToDoDone(Guid id)
        {
            var todoItem = await _documentContext.ToDos.FindAsync(id);

            if(todoItem == null) { return; }

            todoItem.Done = true;
            _documentContext.Update(todoItem);
            var rows = await _documentContext.SaveChangesAsync();
        }
        public async Task<DocumentViewModel> GetDocument(Guid id)
        {
            var doc = await _documentContext.Documents.FindAsync(id);
            var mainCat = await _documentContext.Maincategories.FindAsync(doc.MaincategoryId);
            var subCat = await _documentContext.Subcategories.FindAsync(doc.SubcategoryId);

            var metadataList = await _documentContext.Metadata.Where(x => x.DocId == doc.Id).Select(x => new MetadataViewModel { Name = x.Description, Details = x.Details, Id = x.Id }).ToListAsync();
            var todoList = await _documentContext.ToDos.Where(x => x.DocId == doc.Id).Select(x => new ToDoViewModel { Titel = x.Title, Beschreibung = x.Details, Id = x.Id, DocId = x.DocId, Erledigt = x.Done, Faelligkeit = x.DueDate.Value.ToString("dd.MM.yyyy") }).ToListAsync();
            return new DocumentViewModel
            {
                Id = doc.Id,
                Hauptkategorie = mainCat.Name,
                Unterkategorie = subCat.Name,
                Metadaten = metadataList,
                ToDos = todoList,
                File = doc.File,
                Titel = doc.Title
            };
        }
        public async Task DeleteTodo(Guid id)
        {
            var todoItem = await _documentContext.ToDos.FindAsync(id);

            if(todoItem == null){
                return;
            }

            _documentContext.ToDos.Remove(todoItem);

            await _documentContext.SaveChangesAsync();
        }
        public async Task<bool> RemoveMetadata(Guid metaDataid)
        {
            var metadata = await _documentContext.Metadata.FindAsync(metaDataid);

            _documentContext.Metadata.Remove(metadata);

            var result = await _documentContext.SaveChangesAsync();

            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }




        }
        private async Task<List<ToDoViewModel>> SaveToDos(List<ToDoViewModel> todos, Guid docId)
        {
            if (todos == null)
            {
                return new List<ToDoViewModel>();
            }
            foreach (var item in todos)
            {

                var todoItem = await _documentContext.ToDos.FindAsync(item.Id);

                if (todoItem == null)
                {
                    todoItem = new ToDo { Title = item.Titel, Details = item.Beschreibung, DocId = docId, Done = item.Erledigt };
                    if (DateTime.TryParse(item.Faelligkeit, out var date))
                    {
                        todoItem.DueDate = date;
                    }
                    await _documentContext.ToDos.AddAsync(todoItem);
                }
                else
                {
                    todoItem.Details = item.Beschreibung;
                    todoItem.Title = item.Titel;
                    todoItem.Done = item.Erledigt;
                    if (DateTime.TryParse(item.Faelligkeit, out var date))
                    {
                        todoItem.DueDate = date;
                    }
                    _documentContext.ToDos.Update(todoItem);
                }
            }

            await _documentContext.SaveChangesAsync();

            var newMetadataDb =
                await _documentContext.ToDos.Where(x => x.DocId == docId).Select(x => new ToDoViewModel { Titel = x.Title, Beschreibung = x.Details, Id = x.Id, Faelligkeit = x.DueDate.Value.ToString("dd.MM.yyyy"), Erledigt = x.Done }).ToListAsync();
            return newMetadataDb;
        }
        private async Task<List<MetadataViewModel>> SaveMetadata(List<MetadataViewModel> metadata, Guid docId)
        {
            foreach (var item in metadata)
            {

                var metaData = await _documentContext.Metadata.FindAsync(item.Id);

                if (metaData == null)
                {
                    metaData = new DocumentDb.Models.Metadata { Description = item.Name, Details = item.Details, DocId = docId };
                    await _documentContext.Metadata.AddAsync(metaData);
                }
                else
                {
                    metaData.Details = item.Details;
                    _documentContext.Metadata.Update(metaData);
                }
            }

            await _documentContext.SaveChangesAsync();

            var newMetadataDb =
                await _documentContext.Metadata.Where(x => x.DocId == docId).Select(x => new MetadataViewModel { Name = x.Description, Details = x.Details, Id = x.Id }).ToListAsync();
            return newMetadataDb;
        }
        private async Task<Guid> SaveMaincategory(string name)
        {
            var mainCat = await _documentContext.Maincategories.FirstOrDefaultAsync(x => x.Name == name);

            if (mainCat != null)
            {
                return mainCat.Id;
            }
            else
            {
                mainCat = new Maincategory { Name = name };
                await _documentContext.Maincategories.AddAsync(mainCat);
                await _documentContext.SaveChangesAsync();
                return mainCat.Id;
            }
        }
        private async Task<Guid> SaveSubcategory(string name)
        {
            var subCat = await _documentContext.Subcategories.FirstOrDefaultAsync(x => x.Name == name);

            if (subCat != null)
            {
                return subCat.Id;
            }
            else
            {
                subCat = new Subcategory { Name = name };
                await _documentContext.Subcategories.AddAsync(subCat);
                await _documentContext.SaveChangesAsync();
                return subCat.Id;
            }
        }
        private MetadataViewModel GetMetadata(string metadataline)
        {
            var sanitizedline = metadataline.SkipWhile(x => x == '-').ToArray();
            var sanitizedstring = new string(sanitizedline);
            var metadataarray = sanitizedstring.Split(':');

            return metadataarray == null || metadataarray.Length < 2 ? new MetadataViewModel() :
            new MetadataViewModel
            {
                Name = metadataarray[0].Trim(),
                Details = metadataarray[1].Trim()
            };
        }
    }
}
