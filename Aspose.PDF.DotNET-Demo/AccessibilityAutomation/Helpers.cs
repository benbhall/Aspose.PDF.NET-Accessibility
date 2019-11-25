using Aspose.Pdf;
using Aspose.Pdf.Text;
using System.IO;

namespace Automation
{
    public static class Helpers
    {
        /// <summary>
        /// Generate a non- PDF/UA compliant PDF for use in demo
        /// </summary>
        /// <param name="outputFilePath">Full output path including filename with extension</param>
        public static void CreateDemoNonCompliantPdfFile(string outputFilePath)
        {
            var pdfDoc = new Document();
            pdfDoc.Pages.Add();
            var pageOne = pdfDoc.Pages[1];

            #region Add image
            int lowerLeftX = 100;
            int lowerLeftY = 680;
            int upperRightX = 200;
            int upperRightY = 780;

            FileStream imageStream = new FileStream("aspose.png", FileMode.Open);
            pageOne.Resources.Images.Add(imageStream);
            pageOne.Contents.Add(new Aspose.Pdf.Operators.GSave());
            Aspose.Pdf.Rectangle rectangle = new Aspose.Pdf.Rectangle(lowerLeftX, lowerLeftY, upperRightX, upperRightY);
            Matrix matrix = new Matrix(new double[] { rectangle.URX - rectangle.LLX, 0, 0, rectangle.URY - rectangle.LLY, rectangle.LLX, rectangle.LLY });
            pageOne.Contents.Add(new Aspose.Pdf.Operators.ConcatenateMatrix(matrix));
            XImage ximage = pageOne.Resources.Images[pageOne.Resources.Images.Count];
            pageOne.Contents.Add(new Aspose.Pdf.Operators.Do(ximage.Name));
            pageOne.Contents.Add(new Aspose.Pdf.Operators.GRestore());
            #endregion

            #region Add text fragments
            var textFragmentOne = new TextFragment("Some large indigo text for our header.");
            textFragmentOne.Position = new Position(100, 800);
            textFragmentOne.TextState.FontSize = 24;
            textFragmentOne.TextState.Font = FontRepository.FindFont("TimesNewRoman");
            textFragmentOne.TextState.ForegroundColor = Aspose.Pdf.Color.FromRgb(System.Drawing.Color.Indigo);

            var textFragmentTwo = new TextFragment("Some smaller blue text in a non-tagged document.");
            textFragmentTwo.Position = new Position(100, 660);
            textFragmentTwo.TextState.FontSize = 12;
            textFragmentTwo.TextState.Font = FontRepository.FindFont("TimesNewRoman");
            textFragmentTwo.TextState.ForegroundColor = Aspose.Pdf.Color.FromRgb(System.Drawing.Color.Blue);

            var textBuilder = new TextBuilder(pageOne);
            textBuilder.AppendText(textFragmentOne);
            textBuilder.AppendText(textFragmentTwo);
            #endregion

            pdfDoc.Save(outputFilePath);
        }

    }
}
