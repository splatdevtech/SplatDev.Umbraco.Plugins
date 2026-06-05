using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;
using SplatDev.Umbraco.Plugins.EmailTemplates.Models;

namespace SplatDev.Umbraco.Plugins.EmailTemplates.Migrations;

public class CreateEmailTemplatesTables(IMigrationContext context, ILogger<CreateEmailTemplatesTables> logger)
    : AsyncMigrationBase(context)
{
    protected override async Task MigrateAsync()
    {
        logger.LogDebug("Running migration {MigrationStep}", "CreateEmailTemplatesTables");

        if (!TableExists(EmailTemplate.TableName))
        {
            Create.Table<EmailTemplate>().Do();
            logger.LogInformation("Created table {Table}", EmailTemplate.TableName);
        }

        if (!TableExists(EmailStyle.TableName))
        {
            Create.Table<EmailStyle>().Do();
            logger.LogInformation("Created table {Table}", EmailStyle.TableName);
        }
    }
}
