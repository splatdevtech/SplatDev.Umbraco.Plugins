using System.Threading.Tasks;
﻿using FormBuilder.Extension.Entities;

using Microsoft.Extensions.Logging;

using Umbraco.Cms.Infrastructure.Migrations;

namespace FormBuilder.Extension.Migrations
{
    public class FormBuilderMigration(IMigrationContext context, ILogger<FormBuilderMigration> logger) : AsyncMigrationBase(context)
    {
        private readonly ILogger<FormBuilderMigration> _logger = logger;

        protected override async Task MigrateAsync()
        {
            _logger.LogDebug("Running migration {MigrationStep}", "AddFormBuilder");
            if (!TableExists(Form.TABLE_NAME))
                Create.Table<Form>().Do();
            else
                _logger.LogDebug("The database table {DbTable} already exists, skipping", Form.TABLE_NAME);

            if (!TableExists(FormField.TABLE_NAME))
                Create.Table<FormField>().Do();
            else
                _logger.LogDebug("The database table {DbTable} already exists, skipping", FormField.TABLE_NAME);

            if (!TableExists(DropdownValue.TABLE_NAME))
                Create.Table<DropdownValue>().Do();
            else
                _logger.LogDebug("The database table {DbTable} already exists, skipping", DropdownValue.TABLE_NAME);

            if (!TableExists(EmailTemplate.TABLE_NAME))
                Create.Table<EmailTemplate>().Do();
            else
                _logger.LogDebug("The database table {DbTable} already exists, skipping", EmailTemplate.TABLE_NAME);

            if (!TableExists(Status.TABLE_NAME))
                Create.Table<Status>().Do();
            else
                _logger.LogDebug("The database table {DbTable} already exists, skipping", Status.TABLE_NAME);

            if (!TableExists(Workflow.TABLE_NAME))
                Create.Table<Workflow>().Do();
            else
                _logger.LogDebug("The database table {DbTable} already exists, skipping", Workflow.TABLE_NAME);
        }
    }
}
