namespace UsbPeripheralsLib
{
    /// <summary>
    /// Represents simplified information about a connected USB storage device.
    /// </summary>
    public class UsbDriveModel
    {
        public string? DriveLetter { get; set; }     // Ie. "E:\"
        public string? VolumeLabel { get; set; }     // Ie. "SKOLENI_DATA"
        public long TotalSizeGb { get; set; }
        public long FreeSpaceGb { get; set; }
        public string DisplayName => $"{DriveLetter} ({VolumeLabel}) - Free: {FreeSpaceGb} GB / {TotalSizeGb} GB";
    }
}
