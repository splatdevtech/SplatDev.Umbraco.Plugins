using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Tools.PackageActions;

public abstract class DataTypeAction : IPackageAction
{
    private readonly IDataTypeService _dataTypeService;

    protected DataTypeAction(IDataTypeService dataTypeService)
    {
        _dataTypeService = dataTypeService;
    }

    public abstract string Name { get; }
    protected abstract string DataTypeName { get; }
    protected abstract string EditorAlias { get; }

    public virtual Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var existing = _dataTypeService.GetByEditorAlias(EditorAlias);
        if (existing?.Any() == true)
            return Task.CompletedTask;

        // VoidEditor takes IDataValueEditorFactory in both Umbraco 13 and 17.
        // Passing null! is safe — VoidEditor never invokes the factory at runtime.
        var editor = new VoidEditor((IDataValueEditorFactory)null!);

#if NET10_0_OR_GREATER
        // Umbraco 17: renamed to SystemTextConfigurationEditorJsonSerializer
        var serializer = new global::Umbraco.Cms.Infrastructure.Serialization.SystemTextConfigurationEditorJsonSerializer();
#else
        // Umbraco 13: ConfigurationEditorJsonSerializer
        var serializer = new global::Umbraco.Cms.Infrastructure.Serialization.ConfigurationEditorJsonSerializer();
#endif

        var dataType = new DataType(editor, serializer)
        {
            Name = DataTypeName,
        };

        _dataTypeService.Save(dataType);
        return Task.CompletedTask;
    }
}
