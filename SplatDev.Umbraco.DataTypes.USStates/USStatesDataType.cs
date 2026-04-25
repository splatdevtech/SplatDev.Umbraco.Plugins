#if !NET10_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;

using UmbracoConstants = Umbraco.Cms.Core.Constants;

namespace SplatDev.Umbraco.DataTypes.USStates;

public class USStatesDataType(IServiceScopeFactory scopeFactory)
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private const string DataTypeName = "US States";

    public void Install()
    {
        using var scope = _scopeFactory.CreateScope();
        var dataTypeService = scope.ServiceProvider.GetRequiredService<IDataTypeService>();

        if (dataTypeService.GetDataType(DataTypeName) is not null) return;

        var propertyEditorCollection = scope.ServiceProvider.GetRequiredService<PropertyEditorCollection>();
        var serializer = scope.ServiceProvider.GetRequiredService<IConfigurationEditorJsonSerializer>();

        propertyEditorCollection.TryGet(UmbracoConstants.PropertyEditors.Aliases.DropDownListFlexible, out IDataEditor? editor);
        int counter = 0;
        dataTypeService.Save(new DataType(editor, serializer)
        {
            DatabaseType = ValueStorageType.Ntext,
            CreateDate = DateTime.Now,
            CreatorId = -1,
            Name = DataTypeName,
            Configuration = new DropDownFlexibleConfiguration()
            {
                Multiple = false,
                Items = [
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "ALABAMA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "ALASKA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "AMERICAN SAMOA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "ARIZONA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "ARKANSAS" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "CALIFORNIA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "CAROLINE ISLANDS" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "COLORADO" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "CONNECTICUT" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "DELAWARE" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "DISTRICT OF COLUMBIA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "FEDERATED STATE" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "FLORIDA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "GEORGIA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "GUAM" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "HAWAII" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "IDAHO" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "ILLINOIS" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "INDIANA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "IOWA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "KANSAS" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "KENTUCKY" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "LOUISIANA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "MAINE" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "MARIANA ISLANDS" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "MARSHALL ISLANDS" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "MARYLAND" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "MASSACHUSETTS" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "MICHIGAN" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "MINNESOTA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "MISSISSIPPI" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "MISSOURI" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "MONTANA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "NEBRASKA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "NEVADA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "NEW HAMPSHIRE" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "NEW JERSEY" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "NEW MEXICO" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "NEW YORK" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "NORTH CAROLINA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "NORTH DAKOTA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "NORTHWEST TERRITORIES" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "NUNAVUT" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "OHIO" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "OKLAHOMA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "OREGON" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "PENNSYLVANIA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "PUERTO RICO" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "RHODE ISLAND" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "SOUTH CAROLINA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "SOUTH DAKOTA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "TENNESSEE" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "TEXAS" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "UTAH" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "VERMONT" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "VIRGIN ISLANDS" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "VIRGINIA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "WASHINGTON" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "WEST VIRGINIA" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "WISCONSIN" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "WYOMING" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "YUKON TERRITORY" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "ARMED FORCES - EUROPE" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "ARMED FORCES - AMERICAS" },
                    new ValueListConfiguration.ValueListItem { Id = ++counter, Value = "ARMED FORCES - PACIFIC" }
                ]
            }
        });
    }
}
#else
using Microsoft.Extensions.DependencyInjection;

namespace SplatDev.Umbraco.DataTypes.USStates;

// Umbraco 17 uses Management API for data type creation — needs Umbraco 17 implementation
public class USStatesDataType(IServiceScopeFactory scopeFactory)
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    public void Install()
    {
        // TODO: Implement via Umbraco 17 Management API
    }
}
#endif
