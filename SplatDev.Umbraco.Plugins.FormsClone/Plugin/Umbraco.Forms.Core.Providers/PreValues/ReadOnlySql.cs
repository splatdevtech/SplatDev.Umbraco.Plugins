
// Type: Umbraco.Forms.Core.Providers.PreValues.ReadOnlySql
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System.Runtime.ExceptionServices;

using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Providers.PreValues
{
    public class ReadOnlySql : FieldPreValueSourceType
    {
        private const string Select = "select {0} as [key], {1} as [value], {2} as [caption] from {3} order by {2} ASC";
        private readonly ILogger<ReadOnlySql> _logger;
        private readonly IConfiguration _configuration;

        public ReadOnlySql(ILogger<ReadOnlySql> logger, IConfiguration configuration)
        {
            this.Id = new Guid("F1F5BD4D-E6AE-44ED-86CB-97661E4660B2");
            this.Name = "SQL Database";
            this.Alias = "sqlDatabase";
            this.Description = "Connects to a SQL Server database table and constructs a prevalue source from it";
            this.Icon = "icon-server-alt";
            this._logger = logger;
            this._configuration = configuration;
        }

        [Setting("Connection String", Description = "SQL Server connection string", DisplayOrder = 10, View = "Umb.PropertyEditorUi.TextArea")]
        public virtual string Connection { get; set; } = string.Empty;

        [Setting("Connection String from configuration", Description = "Enter name of SQL Server connection string from configuration path ConnectionStrings.", DisplayOrder = 20)]
        public virtual string ConnectionString { get; set; } = string.Empty;

        [Setting("Table Name", Description = "Table name", DisplayOrder = 30, IsMandatory = true)]
        public virtual string Table { get; set; } = string.Empty;

        [Setting("Key Column", Description = "Column containing the keys", DisplayOrder = 40, IsMandatory = true)]
        public virtual string KeyColumn { get; set; } = string.Empty;

        [Setting("Value Column", Description = "Column containing the values", DisplayOrder = 50, IsMandatory = true)]
        public virtual string ValueColumn { get; set; } = string.Empty;

        [Setting("Caption Column", Description = "Column containing the captions", DisplayOrder = 60)]
        public virtual string CaptionColumn { get; set; } = string.Empty;

        public override async Task<List<PreValue>> GetPreValuesAsync(
          Field? field,
          Form? form)
        {
            List<PreValue> result = new List<PreValue>();
            SqlConnection connection = new SqlConnection(this.GetConnectionString());
            object obj = null;
            int num = 0;
            SqlCommand command;
            List<PreValue> preValuesAsync = [];
            try
            {
                command = connection.CreateCommand();
                try
                {
                    command.CommandText = this.GetCommandText();
                    await connection.OpenAsync().ConfigureAwait(false);
                    try
                    {
                        using (SqlDataReader sqlDataReader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            int num1 = 0;
                            while (sqlDataReader.Read())
                            {
                                result.Add(new PreValue()
                                {
                                    Id = sqlDataReader.GetValue(0)?.ToString() ?? string.Empty,
                                    Value = sqlDataReader.GetString(1),
                                    Caption = sqlDataReader.GetString(2),
                                    SortOrder = num1
                                });
                                ++num1;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, "There was a problem retrieving Form PreValues from SQL Connection trying to run query {SqlQuery}", string.Format("select {0} as [key], {1} as [value], {2} as [caption] from {3} order by {2} ASC", KeyColumn, ValueColumn, Table));
                        throw;
                    }
                    preValuesAsync = result;
                }
                finally
                {
                    ((IDisposable)command)?.Dispose();
                }
                num = 1;
            }
            catch (Exception ex)
            {
                obj = ex;
            }
            if (connection != null)
                await connection.DisposeAsync();
            object obj1 = obj;
            if (obj1 != null)
            {
                if (!(obj1 is Exception source))
                    throw (Exception)obj1;
                ExceptionDispatchInfo.Capture(source).Throw();
            }
            if (num == 1)
                return preValuesAsync;
            obj = null;
            preValuesAsync = null;
            result = null;
            connection = null;
            command = null;
            List<PreValue> preValuesAsync1 = [];
            return preValuesAsync1;
        }

        public override List<Exception> ValidateSettings()
        {
            List<Exception> source = new List<Exception>();
            if (string.IsNullOrEmpty(this.Connection) && string.IsNullOrEmpty(this.ConnectionString))
                source.Add(new Exception("'Connection' setting has not been set"));
            if (string.IsNullOrEmpty(this.Table))
                source.Add(new Exception("'Table' setting has not been set"));
            if (string.IsNullOrEmpty(this.KeyColumn))
                source.Add(new Exception("'Key Column' setting has not been set"));
            if (string.IsNullOrEmpty(this.ValueColumn))
                source.Add(new Exception("'Value Column' setting has not been set"));
            if (source.Any<Exception>())
                return source;
            try
            {
                SqlConnection sqlConnection = new SqlConnection(this.GetConnectionString());
                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandText = this.GetCommandText();
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

        private string GetConnectionString() => !string.IsNullOrEmpty(this.Connection) ? this.Connection : this._configuration.GetConnectionString(this.ConnectionString) ?? string.Empty;

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
