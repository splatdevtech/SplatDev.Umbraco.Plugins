
// Type: Umbraco.Forms.Core.ExportType
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Hosting;
using Microsoft.Win32;

using System.Runtime.Serialization;

using Umbraco.Forms.Core.Interfaces;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Providers;


#nullable enable
namespace Umbraco.Forms.Core
{
    public abstract class ExportType : ProviderBase, IExportType
    {
        protected ExportType(IHostEnvironment hostEnvironment) => this.HostEnvironment = hostEnvironment;

        protected IHostEnvironment HostEnvironment { get; }

        public virtual string MimeType
        {
            get
            {
                string mimeType = "application/octetstream";
                if (OperatingSystem.IsWindows())
                {
                    RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(this.FileExtension);
                    if (registryKey != null)
                    {
                        object obj = registryKey.GetValue("Content Type");
                        if (obj != null)
                            mimeType = obj.ToString();
                    }
                }
                return mimeType;
            }
        }

        [DataMember(Name = "icon")]
        public new string Icon { get; set; } = string.Empty;

        [DataMember(Name = "fileExtension")]
        public string FileExtension { get; set; } = string.Empty;

        [DataMember(Name = "isOsx")]
        public bool IsOsx { get; set; }

        public abstract Task<string> ExportRecordsAsync(Guid formId, RecordExportFilter filter);

        public virtual async Task<string> ExportToFileAsync(
          Guid formId,
          RecordExportFilter filter,
          string filepath)
        {
            if (!Path.IsPathFullyQualified(filepath))
                throw new ArgumentException("The path must be fully qualified.", nameof(filepath));
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            else
            {
                string directoryName = Path.GetDirectoryName(filepath);
                if (directoryName != null)
                    Directory.CreateDirectory(directoryName);
            }
            using (FileStream fs = File.Create(filepath))
                await this.ExportToSteamAsync(formId, filter, fs);
            return filepath;
        }

        protected virtual async Task ExportToSteamAsync(
          Guid formId,
          RecordExportFilter filter,
          Stream stream)
        {
            string str = await this.ExportRecordsAsync(formId, filter).ConfigureAwait(false);
            using (StreamWriter sw = new StreamWriter(stream))
                await sw.WriteAsync(str);
        }
    }
}
