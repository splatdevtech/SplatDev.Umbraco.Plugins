namespace SplatDev.Umbraco.Plugins.CodeFirst
{
    using Microsoft.Extensions.Logging;

    using Umbraco.Cms.Infrastructure.Migrations;

    public class InitialMigration : MigrationBase
    {
        private const string tableName = "pluginApiKeys";

        public InitialMigration(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            Logger.LogDebug("Running Initial Migration with Api Keys");

            if (!TableExists(tableName))
            {
                //this.Create.Table<ApiKey>().Do();
            }
        }
    }
}
