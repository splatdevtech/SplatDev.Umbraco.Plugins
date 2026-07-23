using FormBuilder.Core.Models;
using FormBuilder.Core.Storage.Interfaces;

using System.Data;
using System.Text;

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Scoping;

namespace FormBuilder.Core.Storage
{
#pragma warning disable CS0618 // Type or member is obsolete

    public class PreValueTextFileSystemStorage(
      IFileSystem fileSystem,
      IScopeProvider scopeProvider,
      string? folderName) : IPreValueTextFileStorage
    {
        private readonly IFileSystem _fileSystem = fileSystem;
        private readonly IScopeProvider _scopeProvider = scopeProvider;
        private readonly string _folderName = folderName ?? string.Empty;

        public void DeleteTextFile(string filePath)
        {
            using var scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            _fileSystem.DeleteFile(filePath);
            string? directoryName = Path.GetDirectoryName(filePath);
            string? fileName = Path.GetFileName(directoryName);
            if (!string.IsNullOrEmpty(directoryName) && Guid.TryParse(fileName, out Guid _))
                _fileSystem.DeleteDirectory(directoryName);
            ((ICoreScope)scope).Complete();
        }

        public string GenerateFilePath(string filename, Guid preValueId) => Path.Combine(_folderName, preValueId.ToString(), filename);

        public List<Prevalue> GetTextFilePreValues(string filePath)
        {
            List<Prevalue> textFilePreValues = [];
            using (StreamReader streamReader = new(_fileSystem.OpenFile(filePath)))
            {
                int num = 0;
                string? str;
                while ((str = streamReader.ReadLine()) is not null)
                {
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        string[] strArray = str.Split(['|'], 2);
                        textFilePreValues.Add(new Prevalue()
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
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            _fileSystem.AddFile(GenerateFilePath(filename, preValueId), fileContentStream);
            ((ICoreScope)scope).Complete();
        }

        public void SaveValuesIntoFile(List<string> values, string filePath)
        {
            using IScope scope = _scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, false);
            using (MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, values))))
                _fileSystem.AddFile(filePath, memoryStream, true);
            ((ICoreScope)scope).Complete();
        }
    }

#pragma warning restore CS0618 // Type or member is obsolete
}