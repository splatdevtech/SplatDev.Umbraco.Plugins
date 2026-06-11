namespace SplatDev.Plugins.BackupVault.Models
{
    public class PrismDriveFileUploadRequest
    {
        public byte[] File { get; set; } = [];
        public string FileBase64 { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public string RelativePath { get; set; } = string.Empty;
    }
}
