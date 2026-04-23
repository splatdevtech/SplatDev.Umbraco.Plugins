
// Type: Umbraco.Forms.Data.FileSystem.FormsFileSystemWrapper
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.IO;
using Umbraco.Cms.Core.IO;


#nullable enable
namespace Umbraco.Forms.Data.FileSystem
{
  public abstract class FormsFileSystemWrapper : IFileSystem
  {
    private readonly IFileSystem _inner;

    protected FormsFileSystemWrapper(IFileSystem inner) => this._inner = inner;

    public IEnumerable<string> GetDirectories(string path) => this._inner.GetDirectories(path);

    public void DeleteDirectory(string path) => this._inner.DeleteDirectory(path);

    public void DeleteDirectory(string path, bool recursive) => this._inner.DeleteDirectory(path, recursive);

    public bool DirectoryExists(string path) => this._inner.DirectoryExists(path);

    public void AddFile(string path, Stream stream) => this._inner.AddFile(path, stream);

    public void AddFile(string path, Stream stream, bool overrideIfExists) => this._inner.AddFile(path, stream, overrideIfExists);

    public IEnumerable<string> GetFiles(string path) => this._inner.GetFiles(path);

    public IEnumerable<string> GetFiles(string path, string filter) => this._inner.GetFiles(path, filter);

    public Stream OpenFile(string path) => this._inner.OpenFile(path);

    public void DeleteFile(string path) => this._inner.DeleteFile(path);

    public bool FileExists(string path) => this._inner.FileExists(path);

    public string GetRelativePath(string fullPathOrUrl) => this._inner.GetRelativePath(fullPathOrUrl);

    public string GetFullPath(string path) => this._inner.GetFullPath(path);

    public string GetUrl(string? path) => this._inner.GetUrl(path);

    public DateTimeOffset GetLastModified(string path) => this._inner.GetLastModified(path);

    public DateTimeOffset GetCreated(string path) => this._inner.GetCreated(path);

    public long GetSize(string path) => this._inner.GetSize(path);

    public void AddFile(string path, string physicalPath, bool overrideIfExists = true, bool copy = false) => this._inner.AddFile(path, physicalPath, overrideIfExists, copy);

    public bool CanAddPhysical => this._inner.CanAddPhysical;
  }
}
