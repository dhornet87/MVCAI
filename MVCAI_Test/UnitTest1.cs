using MVCAI.Models;
using MVCAI.Services;

namespace MVCAI_Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GPTConfigTest()
        {
            var ocrengine = new OCRService();
            byte[] doc = File.ReadAllBytes("./data/rechnung.tiff");
            
            var result = ocrengine.ScanDocument(doc);
            string response = await OpenAIModel.QueryGPT(result, "Rechnung");

            Assert.NotNull(result);
            Assert.NotNull(response);
        }
    }
}