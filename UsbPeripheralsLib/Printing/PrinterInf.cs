namespace UsbPeripheralsLib.Printing;

/// <summary>
/// Represents information about a printer found in the system.
/// </summary>
public class PrinterInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string PortName { get; set; } = string.Empty;
}
