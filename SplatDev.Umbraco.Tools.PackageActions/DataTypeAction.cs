using Umbraco.Cms.Core.Models;
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

        var dataType = new DataType(
            new Umbraco.Cms.Core.PropertyEditors.VoidEditor(
                Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance),
            new Umbraco.Cms.Infrastructure.Serialization.SystemTextJsonSerializer())
        {
            Name = DataTypeName,
        };

        _dataTypeService.Save(dataType);
        return Task.CompletedTask;
    }
}
