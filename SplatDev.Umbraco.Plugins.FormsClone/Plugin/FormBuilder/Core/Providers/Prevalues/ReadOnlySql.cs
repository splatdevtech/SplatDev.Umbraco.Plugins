using FormBuilder.Core.Attributes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Prevalues;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FormBuilder.Core.Providers.Prevalues
{
    /// <summary>
    /// Defines a     /// </summary>
    public class ReadOnlySql : FieldPrevalueSourceType
    {
        private const string Select = "select {0} as [key], {1} as [value], {2} as [caption] from {3} order by {2} ASC";
        private readonly ILogger<ReadOnlySql> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public ReadOnlySql(ILogger<ReadOnlySql> logger, IConfiguration configuration)
        {
            Id = new Guid("F1F5BD4D-E6AE-44ED-86CB-97661E4660B2");
            Name = "SQL Database";
            Alias = "sqlDatabase";
            Description = "Connects to a SQL Server database table and constructs a prevalue source from it";
            Icon = "icon-server-alt";
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>Gets or sets the connection string.</summary>
        [Setting("Connection String", Description = "SQL Server connection string", DisplayOrder = 10, View = "Umb.PropertyEditorUi.TextArea")]
        public virtual string Connection { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the connection string key defined in web.config.
        /// </summary>
        [Setting("Connection String from configuration", Description = "Enter name of SQL Server connection string from configuration path ConnectionStrings.", DisplayOrder = 20)]
        public virtual string ConnectionString { get; set; } = string.Empty;

        /// <summary>Gets or sets the table name.</summary>
        [Setting("Table Name", Description = "Table name", DisplayOrder = 30, IsMandatory = true)]
        public virtual string Table { get; set; } = string.Empty;

        /// <summary>Gets or sets the key column.</summary>
        [Setting("Key Column", Description = "Column containing the keys", DisplayOrder = 40, IsMandatory = true)]
        public virtual string KeyColumn { get; set; } = string.Empty;

        /// <summary>Gets or sets the value column.</summary>
        [Setting("Value Column", Description = "Column containing the values", DisplayOrder = 50, IsMandatory = true)]
        public virtual string ValueColumn { get; set; } = string.Empty;

        /// <summary>Gets or sets the caption column.</summary>
        [Setting("Caption Column", Description = "Column containing the captions", DisplayOrder = 60)]
        public virtual string CaptionColumn { get; set; } = string.Empty;

        /// <inheritdoc />
        public override async Task<List<Prevalue>> GetPreValuesAsync(
          Field? field,
          Form? form)
        {
            List<Prevalue>? result = [];
            SqlCommand? command;
            await using SqlConnection connection = new(GetConnectionString());
            command = connection.CreateCommand();
            try
            {
                command.CommandText = GetCommandText();
                await connection.OpenAsync().ConfigureAwait(false);
                try
                {
                    using SqlDataReader sqlDataReader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                    int num = 0;
                    while (sqlDataReader.Read())
                    {
                        result.Add(new Prevalue()
                        {
                            Id = sqlDataReader.GetValue(0)?.ToString() ?? string.Empty,
                            Value = sqlDataReader.GetString(1),
                            Caption = sqlDataReader.GetString(2),
                            SortOrder = num
                        });
                        ++num;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "There was a problem retrieving Form PreValues from SQL Connection trying to run query {SqlQuery}", string.Format("select {0} as [key], {1} as [value], {2} as [caption] from {3} order by {2} ASC", "tablename", KeyColumn, ValueColumn, Table));
                    throw;
                }
                return result;
            }
            finally
            {
                command?.Dispose();
                result = null;
                command = null;
            }
        }

        /// <inheritdoc />
        public override List<Exception> ValidateSettings()
        {
            List<Exception> source = [];
            if (string.IsNullOrEmpty(Connection) && string.IsNullOrEmpty(ConnectionString))
                source.Add(new Exception("'Connection' setting has not been set"));
            if (string.IsNullOrEmpty(Table))
                source.Add(new Exception("'Table' setting has not been set"));
            if (string.IsNullOrEmpty(KeyColumn))
                source.Add(new Exception("'Key Column' setting has not been set"));
            if (string.IsNullOrEmpty(ValueColumn))
                source.Add(new Exception("'Value Column' setting has not been set"));
            if (source.Count != 0)
                return source;
            try
            {
                SqlConnection sqlConnection = new(GetConnectionString());
                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandText = GetCommandText();
                sqlConnection.Open();
                command.ExecuteReader().Dispose();
                sqlConnection.Close();
                sqlConnection.Dispose();
                command.Dispose();
            }
            catch (Exception ex)
            {
                source.Add(ex);
            }
            return source;
        }

        private string GetConnectionString() => !string.IsNullOrEmpty(Connection) ? Connection : _configuration.GetConnectionString(ConnectionString) ?? string.Empty;

        private string GetCommandText()
        {
            var captionColumn = string.IsNullOrWhiteSpace(CaptionColumn)
                ? ValueColumn
                : CaptionColumn;

            return $"SELECT {KeyColumn} AS [key], " +
                   $"{ValueColumn} AS [value], " +
                   $"{captionColumn} AS [caption] " +
                   $"FROM {Table} " +
                   $"ORDER BY [caption] ASC";
        }
    }
}