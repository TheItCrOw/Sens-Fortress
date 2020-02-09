using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SensFortress.Utility
{
    public static class PDF
    {
        public static void CreateNew()
        {
            PdfDocument pdf = new PdfDocument();
            pdf.Info.Title = "Sen's fortress passwords.";
            PdfPage pdfPage = pdf.AddPage();
            XGraphics graph = XGraphics.FromPdfPage(pdfPage);
            XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
            graph.DrawString("This is my first PDF document", font, XBrushes.Black, new XRect(0, 0, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.Center);
            string pdfFilename = $"{IOPathHelper.GetDesktopPath()}\\Passwords.pdf";
            pdf.Save(pdfFilename);
            Process.Start(pdfFilename);
        }

    }
}
