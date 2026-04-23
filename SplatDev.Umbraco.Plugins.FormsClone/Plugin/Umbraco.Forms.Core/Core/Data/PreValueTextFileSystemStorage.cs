
// Type: Umbraco.Forms.Core.Data.PreValueTextFileSystemStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Data;
using System.Text;

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Forms.Core.Models;

using IScope = Umbraco.Cms.Infrastructure.Scoping.IScope;
using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

#nullable enable
namespace Umbraco.Forms.Core.Data
{
    public class PreValueTextFileSystemStorage : IPreValueTextFileStorage
    {
        private readonly IFileSystem _fileSystem;
        private readonly IScopeProvider _scopeProvider;
        private readonly string _folderName;

        public PreValueTextFileSystemStorage(
          IFileSystem fileSystem,
          IScopeProvider scopeProvider,
          string? folderName)
        {
            this._fileSystem = fileSystem;
            this._scopeProvider = scopeProvider;
            this._folderName = folderName ?? string.Empty;
        }

        public void DeleteTextFile(string filePath)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                this._fileSystem.DeleteFile(filePath);
                string directoryName = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileName(directoryName);
                if (!string.IsNullOrEmpty(directoryName) && Guid.TryParse(fileName, out Guid _))
                    this._fileSystem.DeleteDirectory(directoryName);
                scope.Complete();
            }
        }

        public string GenerateFilePath(string filename, Guid preValueId) => Path.Combine(this._folderName, preValueId.ToString(), filename);

        public List<PreValue> GetTextFilePreValues(string filePath)
        {
            List<PreValue> textFilePreValues = new List<PreValue>();
            using (StreamReader streamReader = new StreamReader(this._fileSystem.OpenFile(filePath)))
            {
                int num = 0;
                string str;
                while ((str = streamReader.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        string[] strArray = str.Split(new char[1] { '|' }, 2);
                        textFilePreValues.Add(new PreValue()
                        {
                            Id = num.ToString(),
                            Value = strArray[0].Trim(),
                            Caption = strArray.Length > 1 ? strArray[1].Trim() : string.Empty,
                            SortOrder = num
                        });
                        ++num;
                    }
                }
            }
            return textFilePreValues;
        }

        public void SaveTextFile(Stream fileContentStream, string filename, Guid preValueId)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                this._fileSystem.AddFile(this.GenerateFilePath(filename, preValueId), fileContentStream);
                scope.Complete();
            }
        }

        public void SaveValuesIntoFile(List<string> values, string filePath)
        {
            using (IScope scope = this._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
            {
                using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, values))))
                    this._fileSystem.AddFile(filePath, memoryStream, true);
                scope.Complete();
            }
        }
    }
}
