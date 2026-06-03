namespace SplatDev.DigitalBookCurator.Core.ViewModels;

public class BookViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int PageCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
