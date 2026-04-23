
// Type: Umbraco.Forms.Core.Interfaces.IExportType
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Interfaces
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
