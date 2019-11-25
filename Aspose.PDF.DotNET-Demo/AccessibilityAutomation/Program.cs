using Aspose.Pdf;
using Aspose.Pdf.LogicalStructure;
using Aspose.Pdf.Tagged;
using Aspose.Pdf.Text;
using System.Drawing.Imaging;
using System.IO;

namespace Automation
{
    class Program
    {
        static void Main()
        {
            var inputFileName = "non-compliant.pdf";
            var outputFileName = "compliant.pdf";

            // Use some helper functions to create an example non-PDf/UA-compliant PDF
            Helpers.CreateDemoNonCompliantPdfFile(inputFileName);
            using (var d = new Document(inputFileName))
            {
                bool isValid = d.Validate("non-compliant-validation-log.xml", Aspose.Pdf.PdfFormat.PDF_UA_1);
            }

            var originalDocument = new Document(inputFileName);
            var pageOne = originalDocument.Pages[1];

            // Create our new tagged document
            var taggedDocument = new Document();
            ITaggedContent taggedContent = taggedDocument.TaggedContent;
            StructureElement rootElement = taggedContent.RootElement;

            // Set meta data required by PDF/UA
            taggedContent.SetTitle("Our compliant document.");
            taggedContent.SetLanguage("en-US");

            // Extract from original PDF and create new structured elements for tagged PDF
            HeaderElement h1 = ExtractToHeaderElement(taggedContent, pageOne, 1, 1);
            ParagraphElement p = ExtractToParagraphElement(taggedContent, pageOne, 2);
            FigureElement figureElement = ExtractToFigureElement(taggedContent, pageOne, 1);

            // Append to new tagged content in desired order, which will build structure tree up from the 'root'
            rootElement.AppendChild(h1);
            rootElement.AppendChild(figureElement);
            rootElement.AppendChild(p);

            // Save and validate the compliant PDF
            taggedDocument.Save(outputFileName);
            using (var d = new Document(outputFileName))
            {
                bool isValid = d.Validate("compliant-validation-log.xml", Aspose.Pdf.PdfFormat.PDF_UA_1);
            }
        }

        private static HeaderElement ExtractToHeaderElement(ITaggedContent taggedContent, Page page, int textIndex, int headerLevel = 1)
        {
            TextFragmentAbsorber textFragmentAbsorber = new TextFragmentAbsorber();
            page.Accept(textFragmentAbsorber);
            TextFragment originalHeaderText = textFragmentAbsorber.TextFragments[textIndex];

            HeaderElement h1 = taggedContent.CreateHeaderElement(headerLevel);
            h1.StructureTextState.ForegroundColor = originalHeaderText.TextState.ForegroundColor;
            Font headerFont = FontRepository.FindFont(originalHeaderText.TextState.Font.FontName);
            headerFont.IsEmbedded = true;
            h1.StructureTextState.Font = headerFont;
            h1.SetText(originalHeaderText.Text);

            return h1;
        }

        private static ParagraphElement ExtractToParagraphElement(ITaggedContent taggedContent, Page page, int textIndex)
        {
            TextFragmentAbsorber textFragmentAbsorber = new TextFragmentAbsorber();
            page.Accept(textFragmentAbsorber);

            TextFragment originalText = textFragmentAbsorber.TextFragments[textIndex];

            ParagraphElement p = taggedContent.CreateParagraphElement();
            p.StructureTextState.ForegroundColor = originalText.TextState.ForegroundColor;
            Font paraFont = FontRepository.FindFont(originalText.TextState.Font.FontName);
            paraFont.IsEmbedded = true;
            p.StructureTextState.Font = paraFont;
            p.SetText(originalText.Text);

            return p;
        }

        private static FigureElement ExtractToFigureElement(ITaggedContent taggedContent, Page page, int imageIndex)
        {
            ImagePlacementAbsorber imagePlacementAbsorber = new ImagePlacementAbsorber();
            page.Accept(imagePlacementAbsorber);
            XImage xImage = imagePlacementAbsorber.ImagePlacements[imageIndex].Image;

            FileStream outputImage = new FileStream("temp-image.png", FileMode.Create);
            xImage.Save(outputImage, ImageFormat.Png);
            outputImage.Close();

            FigureElement figureElement = taggedContent.CreateFigureElement();
            figureElement.SetImage("temp-image.png");
            figureElement.AlternativeText = "Aspose logo";

            return figureElement;
        }
    }
}
