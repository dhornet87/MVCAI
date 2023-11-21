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
        public void Test1()
        {
            var ocrengine = new OCRService();
            byte[] doc = File.ReadAllBytes("./data/rechnung.tiff");
            
            var result = ocrengine.ScanDocument(doc);
            Assert.NotNull(result);
        }
    }
}