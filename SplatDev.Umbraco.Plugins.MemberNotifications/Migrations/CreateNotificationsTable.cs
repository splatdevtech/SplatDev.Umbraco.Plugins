using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;
using SplatDev.Umbraco.Plugins.MemberNotifications.Models;

namespace SplatDev.Umbraco.Plugins.MemberNotifications.Migrations;

public class CreateNotificationsTable(IMigrationContext context, ILogger<CreateNotificationsTable> logger)
    : AsyncMigrationBase(context)
{
    protected override async Task MigrateAsync()
    {
        logger.LogDebug("Running migration {MigrationStep}", "CreateNotificationsTable");

        if (!TableExists(MemberNotification.TableName))
        {
            Create.Table<MemberNotification>().Do();
            logger.LogInformation("Created table {Table}", MemberNotification.TableName);
        }
    }
}
