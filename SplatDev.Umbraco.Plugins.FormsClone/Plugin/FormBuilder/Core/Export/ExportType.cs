using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers;

using Microsoft.Extensions.Hosting;
using Microsoft.Win32;

using System.Runtime.Serialization;

using Umbraco.Cms.Core.Extensions;

using File = System.IO.File;

namespace FormBuilder.Core.Export
{
    public abstract class ExportType(IHostEnvironment hostEnvironment) : ProviderBase, IExportType
    {
        protected IHostEnvironment HostEnvironment { get; } = hostEnvironment;

        public virtual string MimeType
        {
            get
            {
                string? mimeType = "application/octetstream";
                if (OperatingSystem.IsWindows())
                {
                    RegistryKey? registryKey = Registry.ClassesRoot.OpenSubKey(FileExtension);
                    if (registryKey is not null)
                    {
                        object? obj = registryKey.GetValue("Content Type");
                        if (obj is not null)
                            mimeType = obj.ToString();
                    }
                }
                return mimeType!;
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
            string str = await ExportRecordsAsync(formId, filter).ConfigureAwait(false);
            string path1 = filepath;
            if (!filepath.Contains('\\'))
                path1 = HostEnvironment.MapPathContentRoot(filepath);
            string path2 = filepath[..filepath.LastIndexOf('\\')];
            if (!path1.EndsWith("." + FileExtension))
                path1 = path1 + "." + FileExtension;
            if (!Directory.Exists(path2))
                Directory.CreateDirectory(path2);
            if (File.Exists(path1))
                File.Delete(path1);
            StreamWriter text = File.CreateText(path1);
            text.Write(str);
            text.Flush();
            text.Close();
            text.Dispose();
            return path1;
        }
    }
}