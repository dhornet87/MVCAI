
using DocumentDb;
using DocumentDb.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Tesseract;

namespace MVCAI.Models
{
    public class DocumentModel(DocumentDbContext documentContext)
    {
        private DocumentDbContext _documentContext => documentContext;

        public async Task<List<DocumentViewModel>> GetDocuments()
        {
            var list = await _documentContext.Documents.Include(x => x.Maincategory).Include(x => x.Subcategory).Include(x => x.Metadatas).ToListAsync();
            var returnlist = new List<DocumentViewModel>();
            foreach (var item in list)
            {
               returnlist.Add( new DocumentViewModel
                {
                    Id = item.Id,
                    Maincategory = item.Maincategory.Name,
                    Subcategory = item.Subcategory.Name,
                    Metadata = item.Metadatas.Select(x => new MetadataViewModel { Description = x.Description, Details = x.Details, Id = x.Id }).ToList()
                });
            }

            return returnlist;


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
                    newDoc.Maincategory = linearray[i].Split(':')[1];
                }
                if (linearray[i].StartsWith("Unterkategorie"))
                {
                    newDoc.Subcategory = linearray[i].Split(':')[1];
                }

                if (linearray[i].StartsWith('-'))
                {
                    metadata.Add(GetMetadata(linearray[i]));
                }
            }

            newDoc.Metadata = metadata;


            return newDoc;
        }
       

        public async Task<DocumentViewModel> Save(DocumentViewModel documentVM)
        {
            var mainCatId = await SaveMaincategory(documentVM.Maincategory);
            var subCatId = await SaveSubcategory(documentVM.Subcategory);

            var doc = await _documentContext.Documents.FindAsync(documentVM.Id);

            if(doc == null)
            {
                
                doc = new Document { MaincategoryId = mainCatId, SubcategoryId = subCatId, File = documentVM.File };
                await _documentContext.AddAsync(doc);

                await _documentContext.SaveChangesAsync();
            }

            documentVM.Id = doc.Id;

            documentVM.Metadata = await SaveMetadata(documentVM.Metadata, documentVM.Id);

            return documentVM;

        }

        public async Task<DocumentViewModel> GetDocument(Guid id)
        {
            var doc = await _documentContext.Documents.FindAsync(id);
            var mainCat = await _documentContext.Maincategories.FindAsync(doc.MaincategoryId);
            var subCat = await _documentContext.Subcategories.FindAsync(doc.SubcategoryId);

            var metadataList = await _documentContext.Metadata.Where(x => x.DocId == doc.Id).Select(x => new MetadataViewModel { Description = x.Description, Details = x.Details, Id = x.Id }).ToListAsync();
            return new DocumentViewModel
            {
                Id = doc.Id,
                Maincategory = mainCat.Name,
                Subcategory = subCat.Name,
                Metadata = metadataList,
                File = doc.File
            };
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
        private async Task<List<MetadataViewModel>> SaveMetadata(List<MetadataViewModel> metadata, Guid docId)
        {
            foreach (var item in metadata)
            {
                
                var metaData = await _documentContext.Metadata.FindAsync(item.Id);

                if(metaData == null)
                {
                    metaData = new Metadata { Description = item.Description, Details = item.Details, DocId = docId };
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
                await _documentContext.Metadata.Where(x => x.DocId == docId).Select(x => new MetadataViewModel { Description = x.Description, Details = x.Details, Id = x.Id }).ToListAsync();
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
                Description = metadataarray[0].Trim(),
                Details = metadataarray[1].Trim()
            };
        }
    }
}
