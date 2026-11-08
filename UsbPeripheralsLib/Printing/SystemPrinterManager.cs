using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace UsbPeripheralsLib.Printing;

/// <summary>
/// Provides multiplatform management, detection, and printing via system print queues.
/// </summary>
[SupportedOSPlatform("windows")] // <--- Indication that the class uses a platform-dependent API
public class SystemPrinterManager(ILogger<SystemPrinterManager>? logger = null)
{
    private readonly ILogger<SystemPrinterManager> _logger = logger ?? NullLogger<SystemPrinterManager>.Instance;

    /// <summary>
    /// Automatically searches for and returns a list of all printers available in the operating system.
    /// </summary>
    public List<PrinterInfo> GetAvailablePrinters()
    {
        var printerList = new List<PrinterInfo>();

        try
        {
            _logger.LogInformation("Searching for available printers in the system...");

            // Getting the default printer (if it exists)
            string defaultPrinterName = string.Empty;
            try
            {
                var settings = new PrinterSettings();
                defaultPrinterName = settings.PrinterName;
            }
            catch
            {
                // In some Linux/headless environments, the default printer may not exist.
            }

            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                var info = new PrinterInfo
                {
                    Name = printerName,
                    IsDefault = (printerName == defaultPrinterName)
                };

                printerList.Add(info);
                _logger.LogInformation("Printer found: {Name} (Default: {Default})", info.Name, info.IsDefault);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading the list of system printers.");
        }

        return printerList;
    }

    /// <summary>
    /// Prints a text document to the selected printer, based on its exact name in the system.
    /// </summary>
    public bool PrintTextToSystemPrinter(string printerName, string textToPrint)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(printerName))
            {
                _logger.LogWarning("The printer name was not entered.");
                return false;
            }

            _logger.LogInformation("Sending print job to system printer '{PrinterName}'...", printerName);

            using var printDoc = new PrintDocument();
            printDoc.PrinterSettings.PrinterName = printerName;

            if (!printDoc.PrinterSettings.IsValid)
            {
                _logger.LogWarning("The printer '{Printer}' is not valid or available in the system.", printerName);
                return false;
            }

            printDoc.PrintPage += (sender, e) =>
            {
                var font = new Font("Arial", 10);
                var brush = Brushes.Black;
                
                float yPos = 50;
                float leftMargin = 50;

                e.Graphics?.DrawString(textToPrint, font, brush, leftMargin, yPos, new StringFormat());
                e.HasMorePages = false;
            };

            printDoc.Print();
            _logger.LogInformation("The print job was successfully submitted to the system print queue.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error printing to system printer '{PrinterName}'.", printerName);
            return false;
        }
    }

    /// <summary>
    /// Prints an image (photo, graphic, logo, scanned document) to the selected system printer.
    /// </summary>
    public bool PrintImage(string printerName, string imageFilePath, bool fitToPage = true)
    {
        try
        {
            if (!File.Exists(imageFilePath))
            {
                _logger.LogWarning("Image file for printing was not found: {Path}", imageFilePath);
                return false;
            }

            _logger.LogInformation("Preparing to print image '{Path}' on printer '{Printer}'...", imageFilePath, printerName);

            using var printDoc = new PrintDocument();
            printDoc.PrinterSettings.PrinterName = printerName;

            if (!printDoc.PrinterSettings.IsValid)
            {
                _logger.LogWarning("The printer '{Printer}' is not valid or available.", printerName);
                return false;
            }

            using var image = Image.FromFile(imageFilePath);

            printDoc.PrintPage += (sender, e) =>
            {
                if (e.Graphics == null) return;

                Rectangle printableArea = e.MarginBounds;

                if (fitToPage)
                {
                    float aspectRatio = (float)image.Width / image.Height;
                    int printWidth = printableArea.Width;
                    int printHeight = (int)(printWidth / aspectRatio);

                    e.Graphics.DrawImage(image, printableArea.Left, printableArea.Top, printWidth, printHeight);
                }
                else
                {
                    e.Graphics.DrawImage(image, printableArea.Left, printableArea.Top);
                }

                e.HasMorePages = false;
            };

            printDoc.Print();
            _logger.LogInformation("Image was successfully dispatched to the print queue.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error printing image to system printer '{PrinterName}'.", printerName);
            return false;
        }
    }
}