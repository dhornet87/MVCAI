﻿using Tesseract;

namespace MVCAI.Services
{
    public class OCRService
    {
        public string ScanDocument(byte[] docbytes)
        {
            using (var engine = new TesseractEngine(@"./data/", "deu", EngineMode.Default))
            {
                using (var img = Pix.LoadFromMemory(docbytes))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();

                        return text;
                        Console.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());

                        Console.WriteLine("Text (GetText): \r\n{0}", text);
                        Console.WriteLine("Text (iterator):");
                        //using (var iter = page.GetIterator())
                        //{
                        //    iter.Begin();

                        //    do
                        //    {
                        //        do
                        //        {
                        //            do
                        //            {
                        //                do
                        //                {
                        //                    if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
                        //                    {
                        //                        Console.WriteLine("<BLOCK>");
                        //                    }

                        //                    Console.Write(iter.GetText(PageIteratorLevel.Word));
                        //                    Console.Write(" ");

                        //                    if (iter.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word))
                        //                    {
                        //                        Console.WriteLine();
                        //                    }
                        //                } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));

                        //                if (iter.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine))
                        //                {
                        //                    Console.WriteLine();
                        //                }
                        //            } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                        //        } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                        //    } while (iter.Next(PageIteratorLevel.Block));
                        //}
                    }
                }
            }
        }
            //catch (Exception e)
            //{
            //    Trace.TraceError(e.ToString());
            //    Console.WriteLine("Unexpected Error: " + e.Message);
            //    Console.WriteLine("Details: ");
            //    Console.WriteLine(e.ToString());
            //}
}
    }
