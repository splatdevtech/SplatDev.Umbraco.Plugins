using System.Threading.Tasks;
﻿using CsvHelper;
using CsvHelper.Configuration;

using Microsoft.Extensions.Logging;

using System.Globalization;

using Umbraco.Cms.Infrastructure.Migrations;
using SplatDev.Umbraco.Plugins.Countries.Models;

namespace SplatDev.Umbraco.Plugins.Countries.Migrations
{
    internal class CountryMigration(IMigrationContext context, ILogger<CountryMigration> logger) : AsyncMigrationBase(context)
    {
        private readonly ILogger<CountryMigration> _logger = logger;
        private const string _skipping = "The database table {DbTable} already exists, skipping";

        protected override async Task MigrateAsync()
        {
            _logger.LogDebug("Running migration {Migration Step}", "HireologyIntegration");

            if (!TableExists(Country.TABLE_NAME))
            {

                Create.Table<Country>().Do();
                //Seed
                //TODO: set path to country csv
                var csvFilePath = "C:\\Temp\\countries.csv";
                using var reader = new StreamReader(csvFilePath);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
                var countries = csv.GetRecords<Country>().ToList();
                context.Database.InsertBulk(countries);
            }
            else
                _logger.LogDebug(_skipping, Country.TABLE_NAME);

        }
    }
}
