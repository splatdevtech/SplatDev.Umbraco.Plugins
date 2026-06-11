namespace SplatDev.uPlugins.Backups.Models
{
    public class BackupPerformedDetails
    {
        public BackupPerformedDetails()
        {
            FileBackups = [];
            DatabaseBackups = [];
        }
        public Dictionary<string, FileDetails> FileBackups { get; set; }
        public Dictionary<string, FileDetails> DatabaseBackups { get; set; }
    }

    public class FileDetails
    {
        public string? Fullname { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
