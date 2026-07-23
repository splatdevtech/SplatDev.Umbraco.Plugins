using FormBuilder.Core.Models;

namespace FormBuilder.Core.Interfaces
{
    public interface IExportType
    {
        string Alias { get; set; }

        string FileExtension { get; set; }

        string Icon { get; set; }

        string MimeType { get; }

        Task<string> ExportRecordsAsync(Guid formId, RecordExportFilter filter);

        Task<string> ExportToFileAsync(Guid formId, RecordExportFilter filter, string filepath);
    }
}