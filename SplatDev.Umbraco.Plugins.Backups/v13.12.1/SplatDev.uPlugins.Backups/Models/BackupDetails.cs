namespace SplatDev.uPlugins.Backups.Models
{
    public class BackupDetails
    {
        public string? ConnectionString { get; set; }
        public string? Server { get; set; }
        public string? Database { get; set; }
        public string? DestinationPath { get; set; }
        public string? RootPath { get; set; }
        public string? DatabasePath { get; set; }
        public string? FilesPath { get; set; }
    }
}
