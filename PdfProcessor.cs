using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;


namespace PdfVerticalDuplicator
{
    public static class PdfProcessor
    {
        // progressCallback: receives 0..100
        public static void Process(string inputPath, string outputPath, Action<int> progressCallback = null)
        {
            // Open input in import mode
            using (var input = PdfReader.Open(inputPath, PdfDocumentOpenMode.Import))
            using (var output = new PdfDocument())
            {
                int total = (input.PageCount + 1) / 2; // number of output pages
                int done = 0;


                for (int i = 0; i < input.PageCount; i += 2)
                {
                    // source page
                    var src = input.Pages[i];


                    double w = src.Width;
                    double h = src.Height;


                    var outPage = new PdfPage();
                    outPage.Width = w;
                    outPage.Height = h * 2;
                    output.AddPage(outPage);


                    using (var gfx = XGraphics.FromPdfPage(outPage))
                    {
                        // XPdfForm loads the whole document, set the page number
                        var form = XPdfForm.FromFile(inputPath);
                        form.PageNumber = i + 1; // 1-based


                        // Draw bottom copy
                        gfx.DrawImage(form, 0, 0, w, h);


                        // Draw top copy
                        gfx.DrawImage(form, 0, h, w, h);
                    }


                    done++;
                    int percent = (int)(done * 100.0 / Math.Max(1, total));
                    progressCallback?.Invoke(percent);
                }


                output.Save(outputPath);
            }
        }
    }
}