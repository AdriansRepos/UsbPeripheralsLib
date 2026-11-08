namespace UsbPeripheralsLib.Documents;

public interface IPdfDocumentGenerator
{
       /// <summary>
       /// Generates a PDF document with defined content at the specified path. 
       /// </summary>
       /// <param name="outputDirectory">Target folder (local disk or path to USB storage).</param>
       /// <param name="fileName">Name of the output file (e.g., "Training_Confirmation.pdf").</param>       
       /// <param name="title">Document title.</param>
       /// <param name="content">Main text content or message.</param>
       /// <returns>True if the file was created successfully; otherwise, false.</returns>
       bool GeneratePdf(string outputDirectory, string fileName, string title, string content);
}