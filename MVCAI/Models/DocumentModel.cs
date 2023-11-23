
using DocumentDb;
using DocumentDb.Models;

namespace MVCAI.Models
{
    public class DocumentModel(DocumentDbContext documentContext, string gptDocument)
    {
        private DocumentDbContext _documentContext => documentContext;

        private string _documenttext => gptDocument;

        public void CreateDocument()
        {

        }

        private Document ParseGPTText()
        {
            var newDoc = new Document();

            //split text string into words/tokens
            var textarray = _documenttext.Split(' ');

            var kategorie = textarray.Where(x => x == "Hauptkategorie:");

            //var kategorie = textarray.Where(x => x == "Unterkategorie");
            //var subkategorie =

            return newDoc;
        }
    }
}
