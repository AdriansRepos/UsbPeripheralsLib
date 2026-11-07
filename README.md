# UsbPeripheralsLib

A lightweight, production-ready .NET Class Library designed to easily manage USB mass storage devices and perform safe file exports. Built with clean architecture principles and enterprise logging abstractions (`Microsoft.Extensions.Logging`).

## Features

- **Automatic USB Detection:** Scans and filters connected removable drives (USB flash drives, external HDDs).
- **Safe File Export:** Handles writing reports, backups, or logs directly to the root of selected USB media.
- **Enterprise Ready:** Fully integrated with `ILogger` abstraction for seamless integration into modern .NET applications (Console, WPF, WinForms, ASP.NET Core).
- **Zero Heavy Dependencies:** Only relies on native .NET libraries and standard logging abstractions.

## Usage Example

```csharp
using UsbPeripheralsLib;

// Initialize the manager (supports dependency injection or standalone usage)
var usbManager = new UsbManager();

// Get all connected USB drives
var drives = usbManager.GetConnectedUsbDrives();

foreach (var drive in drives)
{
    Console.WriteLine($"Found drive: {drive.DisplayName}");
}

// Export a sample file to a specific drive (e.g., "E:\")
bool success = usbManager.ExportFileToUsb("E:\\", "report.csv", "ID;Name\n1;John Doe");

if (success)
{
    Console.WriteLine("Export completed successfully!");
}
```

## Requirements

.NET 8.0 or higher (compatible with .NET Standard / modern .NET ecosystems).

## Installation

Add the project reference or install via NuGet (once published):
```bash
dotnet add package UsbPeripheralsLib
```