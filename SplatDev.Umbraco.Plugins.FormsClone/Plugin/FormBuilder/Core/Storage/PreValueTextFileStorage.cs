using FormBuilder.Core.FileSystem;

using Umbraco.Cms.Core.Scoping;

namespace FormBuilder.Core.Storage
{
#pragma warning disable CS0618 // Type or member is obsolete

    internal sealed class PreValueTextFileStorage(
      FormsFileSystemForSavedData fileSystem,
      IScopeProvider scopeProvider) : PreValueTextFileSystemStorage(fileSystem, scopeProvider, "PreValueTextFiles")
    {
    }

#pragma warning restore CS0618 // Type or member is obsolete
}