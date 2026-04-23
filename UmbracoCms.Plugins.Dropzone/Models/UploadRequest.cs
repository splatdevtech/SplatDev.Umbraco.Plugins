namespace UmbracoCms.Plugins.Dropzone.Models;

public class UploadRequest
{
    public string FolderName { get; set; } = "";
    public int? ParentMediaId { get; set; }
}
