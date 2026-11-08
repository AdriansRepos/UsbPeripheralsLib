using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Reflection.Metadata;

namespace UsbPeripheralsLib.Documents;

/// <summary>
/// Handles the generation and export of PDF documents to specified storage locations.
/// </summary>
public class PdfDocumentGenerator(ILogger<PdfDocumentGenerator>? logger = null) : IPdfDocumentGenerator
{
    private readonly ILogger<PdfDocumentGenerator> _logger = logger ?? NullLogger<PdfDocumentGenerator>.Instance;

    static PdfDocumentGenerator()
    {
        // QuestPDF in Community Mode recommends a license setting.
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public bool GeneratePdf(string outputDirectory, string fileName, string title, string content)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                _logger.LogWarning("No valid folder for the PDF was specified.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                _logger.LogWarning("The PDF file name was not specified.");
                return false;
            }

            // If the .pdf extension is missing, we will add it automatically.
            if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".pdf";
            }

            // We will create a folder if it does not already exist.
            Directory.CreateDirectory(outputDirectory);
            string fullPath = Path.Combine(outputDirectory, fileName);

            // We explicitly call QuestPDF.Fluent.Document to avoid a collision with System.Reflection.Metadata.
            QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // Document header
                    page.Header()
                        .Text(title)
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    // Document body
                    page.Content()
                        .PaddingVertical(20)
                        .Column(col =>
                        {
                            col.Item().Text(content);
                        });

                    // Footer with date and page numbering
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generated: ").FontSize(10);
                            x.Span($"{DateTime.Now:dd.MM.yyyy HH:mm} | ").FontSize(10);
                            x.CurrentPageNumber().FontSize(10);
                            x.Span(" / ").FontSize(10);
                            x.TotalPages().FontSize(10);
                        });
                });
            }).GeneratePdf(fullPath);

            _logger.LogInformation("The PDF document '{FileName}' was successfully generated at: {Path}", fileName, fullPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF document '{FileName}'.", fileName);
            return false;
        }
    }
}