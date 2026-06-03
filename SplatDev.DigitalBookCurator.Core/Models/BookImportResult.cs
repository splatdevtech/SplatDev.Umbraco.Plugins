namespace SplatDev.DigitalBookCurator.Core.Models;

public class BookImportResult
{
    public bool Success { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public Book? Book { get; set; }
}
