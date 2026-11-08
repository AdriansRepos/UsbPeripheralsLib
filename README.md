# UsbPeripheralsLib

A lightweight, production-ready .NET Class Library designed to easily manage USB mass storage devices, document generation (PDF), network printing, and multiplatform system printing (text and images). Built with clean architecture principles and enterprise logging abstractions (`Microsoft.Extensions.Logging`).

## Features

- **Automatic USB Detection:** Scans and filters connected removable drives (USB flash drives, external HDDs).
- **Safe File Export:** Handles writing reports, backups, or logs directly to the root of selected USB media.
- **PDF Document Generation:** Built-in modern document creation powered by QuestPDF.
- **Network Printing (RAW):** Direct TCP printing (Port 9100) to network printers or thermal receipt printers via `NetworkPrinterManager`.
- **Multiplatform System Printing:** Automatic system printer detection (`SystemPrinterManager`) across OS, supporting standard text documents and image/graphic printing (`PrintImage`) with automatic aspect ratio fitting.
- **Enterprise Ready:** Fully integrated with `ILogger` abstraction for seamless integration into modern .NET applications (Console, WPF, WinForms, ASP.NET Core).

## Usage Examples

### 1. USB Storage Management & File Export
```csharp
using UsbPeripheralsLib.Storage;

var usbManager = new UsbManager();
var drives = usbManager.GetConnectedUsbDrives();

foreach (var drive in drives)
{
    Console.WriteLine($"Found drive: {drive.DisplayName}");
}

bool success = usbManager.ExportFileToUsb("E:\\", "report.csv", "ID;Name\n1;John Doe");
```

### 2. Network TCP Printing (RAW)
```csharp
using UsbPeripheralsLib.Printing;

var networkPrinter = new NetworkPrinterManager();
bool netSuccess = await networkPrinter.PrintRawAsync("192.168.100.9", "Hello via Network TCP!");
```

### 3. System Printer Detection & Printing (Text & Images)
```csharp
using UsbPeripheralsLib.Printing;

var systemPrinterManager = new SystemPrinterManager();

// Automatically detect available system printers
var printers = systemPrinterManager.GetAvailablePrinters();
foreach (var p in printers)
{
    Console.WriteLine($"Printer: {p.Name} [Default: {p.IsDefault}]");
}

// Print text or images via system print queues
if (printers.Count > 0)
{
    string targetPrinter = printers.First().Name;
    
    // Print text
    systemPrinterManager.PrintTextToSystemPrinter(targetPrinter, "Hello from USBPeripheralsLib System Print!");
    
    // Print images / graphics / photos
    systemPrinterManager.PrintImage(targetPrinter, "path_to_image.jpg", fitToPage: true);
}
```

## Requirements

- .NET 8.0 or higher (compatible with .NET Standard / modern .NET ecosystems).
- .NET 8.0 or higher
- System.Drawing.Common (for image and system printing features)

QuestPDF (for document generation)

## Installation

Add the project reference or install via NuGet (once published):
```bash
dotnet add package UsbPeripheralsLib
```

## Version History

- **v1.0.0:** Initial release.
  - Automatic USB storage device detection (`UsbManager`).
  - Safe file export to root directories of connected removable media.

- **v1.1.0 (Current):** Added full printing and document ecosystem.
  - PDF document generation via QuestPDF (`UsbPeripheralsLib.Documents`).
  - Network RAW TCP printing via `NetworkPrinterManager`.
  - Multiplatform system printer detection and printing (`SystemPrinterManager`) with full support for text documents and image/graphic printing (`PrintImage`).