using Umbraco.Cms.Core.IO;

namespace FormBuilder.Core.FileSystem
{
    public abstract class FormsFileSystemWrapper(IFileSystem inner) : IFileSystem
    {
        private readonly IFileSystem _inner = inner;

        public IEnumerable<string> GetDirectories(string path) => _inner.GetDirectories(path);

        public void DeleteDirectory(string path) => _inner.DeleteDirectory(path);

        public void DeleteDirectory(string path, bool recursive) => _inner.DeleteDirectory(path, recursive);

        public bool DirectoryExists(string path) => _inner.DirectoryExists(path);

        public void AddFile(string path, Stream stream) => _inner.AddFile(path, stream);

        public void AddFile(string path, Stream stream, bool overrideIfExists) => _inner.AddFile(path, stream, overrideIfExists);

        public IEnumerable<string> GetFiles(string path) => _inner.GetFiles(path);

        public IEnumerable<string> GetFiles(string path, string filter) => _inner.GetFiles(path, filter);

        public Stream OpenFile(string path) => _inner.OpenFile(path);

        public void DeleteFile(string path) => _inner.DeleteFile(path);

        public bool FileExists(string path) => _inner.FileExists(path);

        public string GetRelativePath(string fullPathOrUrl) => _inner.GetRelativePath(fullPathOrUrl);

        public string GetFullPath(string path) => _inner.GetFullPath(path);

        public string GetUrl(string? path) => _inner.GetUrl(path);

        public DateTimeOffset GetLastModified(string path) => _inner.GetLastModified(path);

        public DateTimeOffset GetCreated(string path) => _inner.GetCreated(path);

        public long GetSize(string path) => _inner.GetSize(path);

        public void AddFile(string path, string physicalPath, bool overrideIfExists = true, bool copy = false) => _inner.AddFile(path, physicalPath, overrideIfExists, copy);

        public bool CanAddPhysical => _inner.CanAddPhysical;
    }
}