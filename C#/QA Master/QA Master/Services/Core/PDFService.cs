using iText.Kernel.Pdf;
using QA_Master.Models.Core;
using System.IO;

namespace QA_Master.Services.Core
{
    public class PDFService
    {
        //private float margin = 42.0f;

        private FileStream fileStream;
        private PdfWriter pdfWriter;
        private PdfDocument document;
        private DocumentProperties documentProperties;

        public PDFService(string fileName)
        {
            fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            
            pdfWriter = new PdfWriter(fileStream);
            pdfWriter.SetCompressionLevel(9); //BEST_COMPRESSION
            //PDF_VERSION_1_5

            documentProperties = new DocumentProperties();
            document = new PdfDocument(pdfWriter);
        }

        public async Task<bool> Start(string headerPrefix, Workflow workflow, Models.Core.Action action)
        {
            bool result = await Task.Run(() => { return WriteAction(headerPrefix, workflow, action); });

            return result;
        }

        public void End()
        {
            document.Close();
        }

        private bool WriteAction(string headerPrefix, Workflow workflow, Models.Core.Action action)
        {
            if (action.Screenshot == null)
            {
                System.Threading.Thread.Sleep(50);

                return true;
            }

            System.Threading.Thread.Sleep(50);

            //document.NewPage();
            //
            //PdfPTable pdfPTable = new PdfPTable(1);
            //pdfPTable.WidthPercentage = 100;
            ////pdfPTable.HorizontalAlignment = HorizontalAlignment.Left;
            //pdfPTable.DeleteBodyRows();
            //
            //PdfFont contentFont = PdfFontFactory.CreateFont("Verdana"); //10
            ////contentFont.
            //if (action.RunStatus == EnumRunStatus.Passed && (action.Perform == null || action.Perform.RunStatus == EnumRunStatus.Passed))
            //{
            //    contentFont.Color = BaseColor.BLACK;
            //}
            //else
            //{
            //    contentFont.Color = BaseColor.RED;
            //}
            //
            //Paragraph para = new Paragraph($"{headerPrefix} - {workflow}", contentFont);
            //pdfPCell = new PdfPCell(para);
            //pdfPCell.Border = 0; pdfPCell.BorderWidth = 0;
            //pdfPTable.AddCell(pdfPCell);
            //
            //para = new Paragraph(action.GetFullDescription(), contentFont);
            //pdfPCell = new PdfPCell(para);
            //pdfPCell.Border = 0; pdfPCell.BorderWidth = 0;
            //pdfPTable.AddCell(pdfPCell);
            //
            //pdfPCell = new PdfPCell(new Paragraph(" "));
            //pdfPCell.Border = 0; pdfPCell.BorderWidth = 0;
            //pdfPTable.AddCell(pdfPCell);
            //
            //Image image = Image.GetInstance(action.Screenshot);
            //image.CompressionLevel = PdfStream.BEST_COMPRESSION;
            //float ratio = (document.PageSize.Width - 2.1f * margin) / image.Width;
            //image.ScaleAbsolute(image.Width * ratio, image.Height * ratio);
            //pdfPCell = new PdfPCell(image);
            //pdfPCell.Border = 1; pdfPCell.BorderWidth = 1;
            //pdfPTable.AddCell(pdfPCell);
            //
            //document.Add(pdfPTable);

            return true;
        }
    }
}
