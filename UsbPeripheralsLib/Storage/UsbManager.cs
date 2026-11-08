using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace UsbPeripheralsLib.Storage
{
    /// <summary>
    /// Manages detection and file operations for USB mass storage devices.
    /// Initializes a new instance of the <see cref="UsbManager"/> class with optional logging support.    
    /// </summary>
    /// <param name="logger">The logger instance. If null, a safe NullLogger is used.</param>
    public class UsbManager(ILogger<UsbManager>? logger = null)
    {
        private readonly ILogger<UsbManager> _logger = logger ?? NullLogger<UsbManager>.Instance;

        /// <summary>
        /// Searches for all currently connected and ready removable USB drives or external HDDs.
        /// </summary>
        /// <returns>A collection of available USB drives.</returns>
        public IEnumerable<UsbDriveModel> GetConnectedUsbDrives()
        {
            try
            {
                var drives = DriveInfo.GetDrives()
                    .Where(d => d.DriveType == DriveType.Removable && d.IsReady)
                    .Select(d => new UsbDriveModel
                    {
                        DriveLetter = d.Name,
                        VolumeLabel = string.IsNullOrWhiteSpace(d.VolumeLabel) ? "Unnamed" : d.VolumeLabel,
                        TotalSizeGb = d.TotalSize / (1024 * 1024 * 1024),
                        FreeSpaceGb = d.AvailableFreeSpace / (1024 * 1024 * 1024)
                    })
                    .ToList();

                _logger.LogDebug("Successfully detected {Count} connected USB drives.", drives.Count);
                return drives;
            }
            catch (Exception ex)
            {
                // Structured production logging – records an exception, including the stack trace
                _logger.LogError(ex, "Critical error occurred while detecting connected USB drives.");
                return [];
            }
        }

        /// <summary>
        /// Exports a file (e.g., reports, logs, attendance lists) to the specified USB drive root directory.
        /// </summary>
        /// <param name="driveLetter">The target drive letter (e.g., "E:\\").</param>
        /// <param name="fileName">The name of the file to create.</param>
        /// <param name="fileContent">The string content to write into the file.</param>
        /// <returns>True if the file was successfully written; otherwise, false.</returns>
        public bool ExportFileToUsb(string driveLetter, string fileName, string fileContent)
        {
            try
            {
                var drive = new DriveInfo(driveLetter);
                if (!drive.IsReady)
                {
                    _logger.LogWarning("Attempted to export to drive {DriveLetter}, but the drive is not ready.", driveLetter);
                    return false;
                }

                string targetPath = Path.Combine(drive.RootDirectory.FullName, fileName);
                File.WriteAllText(targetPath, fileContent, System.Text.Encoding.UTF8);
                
                _logger.LogInformation("File '{FileName}' was successfully written to USB drive {DriveLetter}.", fileName, driveLetter);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write file '{FileName}' to USB drive {DriveLetter}.", fileName, driveLetter);
                return false;
            }
        }
    }
}