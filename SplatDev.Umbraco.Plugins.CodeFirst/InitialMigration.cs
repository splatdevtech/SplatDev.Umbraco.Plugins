using System.Threading.Tasks;
namespace SplatDev.Umbraco.Plugins.CodeFirst
{
    using Microsoft.Extensions.Logging;

    using Umbraco.Cms.Infrastructure.Migrations;

    public class InitialMigration : AsyncMigrationBase
    {
        private const string tableName = "pluginApiKeys";

        public InitialMigration(IMigrationContext context) : base(context)
        {
        }

        protected override async Task MigrateAsync()
        {
            Logger.LogDebug("Running Initial Migration with Api Keys");

            if (!TableExists(tableName))
            {
                //this.Create.Table<ApiKey>().Do();
            }
        }
    }
}
