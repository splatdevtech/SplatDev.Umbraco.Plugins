using CsvHelper;
using CsvHelper.Configuration;

using Microsoft.Extensions.Logging;

using SplatDev.Umbraco.Plugins.Countries.Models;

using System.Globalization;

using Umbraco.Cms.Infrastructure.Migrations;

namespace SplatDev.Umbraco.Plugins.Countries.Migrations
{
    internal class CountryMigration(IMigrationContext context, ILogger<CountryMigration> logger) : MigrationBase(context)
    {
        private readonly ILogger<CountryMigration> _logger = logger;
        private const string _skipping = "The database table {DbTable} already exists, skipping";

        protected override void Migrate()
        {
            _logger.LogDebug("Running migration {Migration Step}", "HireologyIntegration");

            if (!TableExists(Country.TABLE_NAME))
            {

                Create.Table<Country>().Do();
                //Seed
                var assemblyLocation = Path.GetDirectoryName(typeof(CountryMigration).Assembly.Location) ?? string.Empty;
                var contentRoot = Directory.GetCurrentDirectory();
                var csvCandidates = new[]
                {
                    Path.Combine(contentRoot, "App_Data", "SplatDev.Umbraco.Plugins.Countries", "countries.csv"),
                    Path.Combine(assemblyLocation, "App_Data", "SplatDev.Umbraco.Plugins.Countries", "countries.csv"),
                    Path.Combine(contentRoot, "App_Data", "countries.csv"),
                    Path.Combine(assemblyLocation, "App_Data", "countries.csv")
                };

                var csvFilePath = csvCandidates.FirstOrDefault(File.Exists);

                if (string.IsNullOrWhiteSpace(csvFilePath))
                {
                    _logger.LogError("Countries CSV file not found. Checked paths: {CsvPaths}", string.Join(", ", csvCandidates));
                    throw new FileNotFoundException($"Countries CSV file not found. Checked paths: {string.Join(", ", csvCandidates)}");
                }

                using var reader = new StreamReader(csvFilePath);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null
                });
                var countries = csv.GetRecords<Country>().ToList();
                context.Database.InsertBulk(countries);
            }
            else
                _logger.LogDebug(_skipping, Country.TABLE_NAME);

        }
    }
}
