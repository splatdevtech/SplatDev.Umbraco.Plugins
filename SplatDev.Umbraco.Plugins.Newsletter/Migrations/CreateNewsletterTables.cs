using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;
using SplatDev.Umbraco.Plugins.Newsletter.Models;

namespace SplatDev.Umbraco.Plugins.Newsletter.Migrations;

public class CreateNewsletterTables(IMigrationContext context, ILogger<CreateNewsletterTables> logger)
    : AsyncMigrationBase(context)
{
    protected override async Task MigrateAsync()
    {
        logger.LogDebug("Running migration {MigrationStep}", "CreateNewsletterTables");

        if (!TableExists(SubscriberList.TableName))
        {
            Create.Table<SubscriberList>().Do();
            logger.LogInformation("Created table {Table}", SubscriberList.TableName);
        }

        if (!TableExists(Subscriber.TableName))
        {
            Create.Table<Subscriber>().Do();
            logger.LogInformation("Created table {Table}", Subscriber.TableName);
        }

        if (!TableExists(Campaign.TableName))
        {
            Create.Table<Campaign>().Do();
            logger.LogInformation("Created table {Table}", Campaign.TableName);
        }

        if (!TableExists(CampaignStats.TableName))
        {
            Create.Table<CampaignStats>().Do();
            logger.LogInformation("Created table {Table}", CampaignStats.TableName);
        }
    }
}
