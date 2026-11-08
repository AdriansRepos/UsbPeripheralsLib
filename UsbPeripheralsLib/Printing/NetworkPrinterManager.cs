using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace UsbPeripheralsLib.Printing;

/// <summary>
/// Handles sending print jobs to network (Wi-Fi / Ethernet) printers using TCP sockets (port 9100).
/// </summary>
/// <param name="logger">The logger instance. If null, a safe NullLogger is used.</param>
public class NetworkPrinterManager(ILogger<NetworkPrinterManager>? logger = null)
{
    private readonly ILogger<NetworkPrinterManager> _logger = logger ?? NullLogger<NetworkPrinterManager>.Instance;

    /// <summary>
    /// Sends raw text or data to a network printer.
    /// </summary>
    /// <param name="ipAddress">The IP address of the printer on the local network (e.g., "192.168.1.50").</param>
    /// <param name="port">The printer port (the default for RAW printing is typically 9100).</param>
    /// <param name="dataBytes">A byte array of the data to be printed.</param>
    /// <returns>True if the print job was successfully sent; otherwise, false.</returns>
    public bool SendRawDataToPrinter(string ipAddress, int port, byte[] dataBytes)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                _logger.LogWarning("A valid printer IP address was not entered.");
                return false;
            }

            _logger.LogInformation("Attempting to connect to a network printer at {Ip}:{Port}...", ipAddress, port);

            using var client = new TcpClient();
            // Connection attempt with a timeout (e.g., 3 seconds)
            var connectResult = client.BeginConnect(ipAddress, port, null, null);
            bool success = connectResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3));

            if (!success || !client.Connected)
            {
                _logger.LogWarning("The connection to the network printer {Ip}:{Port} timed out or failed.", ipAddress, port);
                return false;
            }

            using var stream = client.GetStream();
            stream.Write(dataBytes, 0, dataBytes.Length);
            stream.Flush();

            _logger.LogInformation("Print data was successfully sent to printer {Ip}.", ipAddress);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending print data to network printer {Ip}:{Port}.", ipAddress, port);
            return false;
        }
    }

    /// <summary>
    /// Helper method for sending plain text to a network printer.
    /// </summary>
    public bool PrintText(string ipAddress, string text, int port = 9100)
    {
        // Convert the text into bytes using the UTF-8 encoding (this can be changed depending on the printer—for example, to Windows-1250).
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        return SendRawDataToPrinter(ipAddress, port, bytes);
    }

    /// <summary>
    /// A helper method for sending an entire PDF file to a network printer (provided the printer can process PDF natively).
    /// </summary>
    public bool PrintPdfFile(string ipAddress, string pdfFilePath, int port = 9100)
    {
        try
        {
            if (!File.Exists(pdfFilePath))
            {
                _logger.LogWarning("The PDF file for printing was not found at the path: {Path}", pdfFilePath);
                return false;
            }

            byte[] pdfBytes = File.ReadAllBytes(pdfFilePath);
            _logger.LogInformation("PDF file loaded ({Size} bytes) for printing on a network printer.", pdfBytes.Length);
            
            return SendRawDataToPrinter(ipAddress, port, pdfBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading or sending the PDF file to the printer.");
            return false;
        }
    }
}