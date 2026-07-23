using FormBuilder.Core.Attributes;
using FormBuilder.Core.DataSources;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Fields;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Mapping;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Analyzers;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Persistence.Schema;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using System.Data;
using System.Text;

namespace FormBuilder.Core.Providers.DataSourceTypes
{
    /// <summary>
    /// Provides a form datasource type based on a MSSQL Server database table.
    /// </summary>
    public class MsSql : FormDataSourceType, IFieldPrevalueCrudSupport
    {
        private const string InnerJoin = "INNER JOIN [{0}] ON [{0}].[{1}] = [{2}].[{3}]";
        private const string Insert = "INSERT INTO {0}({1}) VALUES({2})";
        private const string Update = "UPDATE {0} SET {1} WHERE {2}";
        private const string Delete = "DELETE FROM {0} WHERE {1}";
        private const string Field = "[{0}].[{1}]";
        private const string Select = "SELECT TOP {0} {1} from {2} {3} {4}";
        private const string Paging = "SELECT *\n                                FROM (SELECT TOP ({0}) *\n                                FROM ({1}) AS newtbl\n                                ORDER BY {2} {3}) AS derivedtbl_1 ORDER BY {2} {4}";
        private readonly IFormService _formService;
        private readonly IFieldTypeStorage _fieldTypeStorage;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<MsSql> _logger;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public MsSql(
          IFormService formService,
          IFieldTypeStorage fieldTypeStorage,
          ILoggerFactory loggerFactory)
        {
            _formService = formService;
            _fieldTypeStorage = fieldTypeStorage;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<MsSql>();
            Id = new Guid("F19506F3-EFEA-4B13-A308-89348F69DF91");
            Name = "SQL Database";
            Alias = "sqlDatabase";
            Description = "Connects to any SQL Server database table and constructs a datasource from it";
            Icon = "icon-server-alt";
        }

        /// <summary>Gets or sets an OleDB Compatible connection string.</summary>
        [Setting("Connection String", Description = "SQL Server connection string", DisplayOrder = 10, IsMandatory = true, View = "Umb.PropertyEditorUi.TextArea")]
        public virtual string Connection { get; set; } = string.Empty;

        /// <summary>Gets or sets a table name.</summary>
        [Setting("Table Name", Description = "Table name", DisplayOrder = 20, IsMandatory = true)]
        public virtual string Table { get; set; } = string.Empty;

        /// <inheritdoc />
        public override List<Record> GetRecords(
          Form form,
          int page,
          int maxItems,
          object sortByField,
          RecordSorting order)
        {
            List<Record> records = [];
            List<FormDataSourceMapping> fieldMappings = form.DataSource?.Mappings ?? [];
            SqlConnection sqlConnection = new(Connection);
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = PagingSql(page, maxItems, sortByField, order, fieldMappings);
            sqlConnection.Open();
            SqlDataReader dr = command.ExecuteReader();
            while (dr is not null && dr.Read())
                records.Add(DataReaderToRecord(dr, form));
            sqlConnection.Close();
            dr?.Dispose();
            command.Dispose();
            return records;
        }

        private static Record DataReaderToRecord(SqlDataReader dr, Form form)
        {
            Record record = new()
            {
                Created = DateTime.Now,
                Updated = DateTime.Now,
                Form = form.Id,
                State = FormState.Submitted
            };
            RecordField recordField = new();
            foreach (Field allField in form.AllFields)
            {
                if (allField.DataSourceFieldKey is not null)
                {
                    FormDataSourceMapping? mapping = form.DataSource?.GetMapping(allField.DataSourceFieldKey);
                    if (mapping is not null)
                    {
                        string prevalueValueField = allField.DataSourceFieldKey.ToString();
                        if (!string.IsNullOrEmpty(mapping.PrevalueValueField))
                            prevalueValueField = mapping.PrevalueValueField;
                        if (dr[prevalueValueField] is not null)
                        {
                            recordField.Field = allField;
                            recordField.Values.Add(dr[prevalueValueField]);
                            recordField.Key = new Guid();
                            record.RecordFields.Add(allField.Id, recordField);
                        }
                    }
                }
            }
            return record;
        }

        /// <inheritdoc />
        public override Record InsertRecord(Record record)
        {
            List<FormDataSourceMapping> mappings = _formService.Get(record.Form)?.DataSource?.Mappings ?? [];
            string str = InsertSql(mappings);
            SqlConnection sqlConnection = new(Connection);
            SqlCommand command = sqlConnection.CreateCommand();
            try
            {
                command.CommandText = str;
                foreach (FormDataSourceMapping dataSourceMapping in mappings)
                {
                    object obj = new();
                    if (string.IsNullOrEmpty(dataSourceMapping.DefaultValue))
                    {
                        foreach (RecordField recordField in record.RecordFields.Values)
                        {
                            if (recordField.Field?.DataSourceFieldKey == dataSourceMapping.DataFieldKey)
                            {
                                FieldType? fieldTypeByField = _fieldTypeStorage.GetFieldTypeByField(recordField.Field);
                                if (fieldTypeByField is not null)
                                    obj = Convert.ChangeType(recordField.ValuesAsString(), fieldTypeByField.GetDataType());
                            }
                        }
                    }
                    else
                        obj = dataSourceMapping.DefaultValue;
                    command.Parameters.AddWithValue("@" + dataSourceMapping.DataFieldKey, obj);
                }
                sqlConnection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was a problem trying to insert record with UniqueID {RecordId} into SQL DataSource Provider Connection for Table: {TableName}", record.UniqueId, Table);
                throw;
            }
            finally
            {
                sqlConnection.Close();
                command.Dispose();
            }
            return record;
        }

        /// <inheritdoc />
        public override Dictionary<object, FormDataSourceField> GetMappedFields() => [];

        /// <inheritdoc />
        public override Dictionary<object, FormDataSourceField> GetAvailableFields()
        {
            MsSqlAnalyzer msSqlAnalyzer = new(Connection, _loggerFactory.CreateLogger<MsSqlAnalyzer>());
            DatabaseTable table = msSqlAnalyzer.GetTable(Table);
            Dictionary<object, FormDataSourceField> availableFields = [];
            if (table.Columns is null) return [];
            foreach (var column in table.Columns as IEnumerable<DatabaseColumn>)
            {
                FormDataSourceField formDataSourceField = new()
                {
                    Name = column.Name,
                    Key = column.Name,
                    IsPrimaryKey = column.IsPrimaryKey,
                    IsForeignKey = column.IsForeignKey
                };
                if (formDataSourceField.IsForeignKey)
                {
                    formDataSourceField.PreValueKeyField = column.PrimaryKeyColumn;
                    formDataSourceField.PreValueSource = column.Schema + "." + column.PrimaryKeyTable;
                    formDataSourceField.AvailablePreValueValueFields = [];
                    var columns = msSqlAnalyzer.GetSqlColumns(column.PrimaryKeyTable);
                    if (columns is null) return [];
                    foreach (DataRow row in columns.Rows as InternalDataCollectionBase)
                    {
                        string? str = row["COLUMN_NAME"]?.ToString();
                        if (!string.IsNullOrEmpty(str))
                            formDataSourceField.AvailablePreValueValueFields.Add(str);
                    }
                }
                formDataSourceField.Type = column.Type;
                formDataSourceField.MaxLength = column.MaxLength;
                formDataSourceField.AutoIncrement = column.AutoIncrement;
                formDataSourceField.AllowNulls = column.AllowNulls;
                formDataSourceField.IsMandatory = true;
                if (column.AllowNulls)
                    formDataSourceField.IsMandatory = false;
                if (column.AutoIncrement)
                {
                    formDataSourceField.IsMandatory = false;
                    formDataSourceField.IsProtected = true;
                }
                if (column.DefaultSetting is not null)
                    formDataSourceField.IsMandatory = false;
                availableFields.Add(column.Name, formDataSourceField);
            }
            msSqlAnalyzer.Dispose();
            return availableFields;
        }

        /// <inheritdoc />
        public override Dictionary<object, string> GetPreValues(Field field, Form form)
        {
            Dictionary<object, string> preValues = [];
            if (field.DataSourceFieldKey is not null)
            {
                FormDataSourceMapping? dataSourceMapping = (form.DataSource?.Mappings ?? []).FirstOrDefault(x => string.Equals(x.DataFieldKey.ToString(), field.DataSourceFieldKey.ToString(), StringComparison.OrdinalIgnoreCase));
                if (dataSourceMapping == null)
                    return preValues;
                SqlConnection sqlConnection = new(Connection);
                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandText = string.Format("SELECT [{1}] AS [val], [{2}] AS [key] FROM {0} ORDER BY {1} ASC", dataSourceMapping.PrevalueTable, dataSourceMapping.PrevalueValueField, dataSourceMapping.PrevalueKeyfield);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = command.ExecuteReader();
                while (sqlDataReader is not null && sqlDataReader.Read())
                    preValues.Add(sqlDataReader["key"], sqlDataReader["val"].ToString()!);
                sqlConnection.Close();
                sqlDataReader?.Dispose();
                sqlConnection.Dispose();
                command.Dispose();
            }
            return preValues;
        }

        /// <inheritdoc />
        public Prevalue InsertValue(Prevalue preValue, Field field, Form form)
        {
            switch (field?.DataSourceFieldKey)
            {
                case null:
                    return preValue;

                default:
                    FormDataSourceMapping? dataSourceMapping = (form.DataSource?.Mappings ?? []).FirstOrDefault(x => x.DataFieldKey == field.DataSourceFieldKey);
                    if (dataSourceMapping == null)
                        return preValue;
                    SqlConnection sqlConnection = new(Connection);
                    SqlCommand command = sqlConnection.CreateCommand();
                    command.CommandText = string.Format("INSERT INTO {0}({1}) VALUES({2})", dataSourceMapping.PrevalueTable, dataSourceMapping.PrevalueValueField, "?");
                    command.Parameters.Add(new SqlParameter("@value", preValue.Value));
                    try
                    {
                        sqlConnection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "There was a problem trying to insert a Prevalue with value {Prevalue} for Field: {FieldName} in Form {FormName} with an id {FormId} into SQL DataSource Provider for Table: {TableName}", preValue.Value, field.Caption, form.Name, form.Id, Table);
                        throw;
                    }
                    finally
                    {
                        sqlConnection.Close();
                        command.Dispose();
                        sqlConnection.Dispose();
                    }
                    return preValue;
            }
        }

        /// <inheritdoc />
        public Prevalue UpdateValue(Prevalue preValue, Field field, Form form)
        {
            if (field is not null && field.DataSourceFieldKey is not null)
            {
                FormDataSourceMapping? dataSourceMapping = (form.DataSource?.Mappings ?? []).FirstOrDefault(x => x.DataFieldKey == field.DataSourceFieldKey);
                if (dataSourceMapping == null)
                    return preValue;
                SqlConnection sqlConnection = new(Connection);
                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandText = string.Format("UPDATE {0} SET {1} WHERE {2}", dataSourceMapping.PrevalueTable, dataSourceMapping.PrevalueValueField + " = ?", dataSourceMapping.PrevalueKeyfield + " = ?");
                command.Parameters.Add(new SqlParameter("@value", preValue.Value));
                command.Parameters.Add(new SqlParameter("@key", preValue.Id));
                try
                {
                    sqlConnection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "There was a problem trying to update a Prevalue with value {Prevalue} for Field: {FieldName} in Form {FormName} with an id {FormId} into SQL DataSource Provider for Table: {TableName}", preValue.Value, field.Caption, form.Name, form.Id, Table);
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    command.Dispose();
                    sqlConnection.Dispose();
                }
            }
            return preValue;
        }

        /// <inheritdoc />
        public bool DeleteValue(Prevalue preValue, Field field, Form form)
        {
            switch (field?.DataSourceFieldKey)
            {
                case null:
                    return true;

                default:
                    FormDataSourceMapping? dataSourceMapping = (form.DataSource?.Mappings ?? []).FirstOrDefault(x => x.DataFieldKey == field.DataSourceFieldKey);
                    if (dataSourceMapping == null)
                        return true;
                    SqlConnection sqlConnection = new(Connection);
                    SqlCommand command = sqlConnection.CreateCommand();
                    command.CommandText = string.Format("DELETE FROM {0} WHERE {1}", dataSourceMapping.PrevalueTable, dataSourceMapping.PrevalueKeyfield + " = ?");
                    command.Parameters.Add(new SqlParameter("@key", preValue.Id));
                    try
                    {
                        sqlConnection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "There was a problem trying to delete a Prevalue with value {Prevalue} for Field: {FieldName} in Form {FormName} with an id {FormId} into SQL DataSource Provider for Table: {TableName}", preValue.Value, field.Caption, form.Name, form.Id, Table);
                        throw;
                    }
                    finally
                    {
                        sqlConnection.Close();
                        command.Dispose();
                        sqlConnection.Dispose();
                    }
                    return true;
            }
        }

        /// <inheritdoc />
        public void SortValues(Guid fieldId, List<Prevalue> values, Field field, Form form)
        {
        }

        private string PagingSql(
            int page,
            int maxItems,
            object sortByField,
            RecordSorting order,
            List<FormDataSourceMapping> fieldMappings)
        {
            var skip = page * maxItems;
            var joins = new StringBuilder();
            var selectedFields = new StringBuilder();

            foreach (var mapping in fieldMappings)
            {
                if (!string.IsNullOrEmpty(mapping.PrevalueValueField) &&
                    !string.IsNullOrEmpty(mapping.PrevalueKeyfield) &&
                    !string.IsNullOrEmpty(mapping.PrevalueTable))
                {
                    selectedFields.Append($"[{mapping.PrevalueTable}].[{mapping.PrevalueValueField}],");
                    joins.Append(
                        $"INNER JOIN [{mapping.PrevalueTable}] " +
                        $"ON [{mapping.PrevalueTable}].[{mapping.PrevalueKeyfield}] " +
                        $"= [{Table}].[{mapping.DataFieldKey}] ");
                }
                else
                {
                    selectedFields.Append($"[{Table}].[{mapping.DataFieldKey}],");
                }
            }

            var fields = selectedFields.ToString().TrimEnd(',');
            var sortDirection = order == RecordSorting.Ascending ? "ASC" : "DESC";
            var reverseSort = order == RecordSorting.Ascending ? "DESC" : "ASC";

            var orderByClause = sortByField is not null
                ? $"ORDER BY {sortByField} {sortDirection}"
                : string.Empty;

            var baseQuery = $"SELECT TOP {skip} {fields} FROM {Table} {joins} {orderByClause}".Trim();

            if (page <= 1)
            {
                return baseQuery;
            }

            return $@"SELECT *
        FROM (
            SELECT TOP ({maxItems}) *
            FROM ({baseQuery}) AS newtbl
            ORDER BY {sortByField} {reverseSort}
        ) AS derivedtbl_1
        ORDER BY {sortByField} {sortDirection}";
        }

        private string InsertSql(List<FormDataSourceMapping> mappings)
        {
            string str1 = string.Empty;
            string str2 = string.Empty;
            foreach (FormDataSourceMapping mapping in mappings)
            {
                str2 = str2 + "[" + mapping.DataFieldKey + "],";
                str1 = str1 + "@" + mapping.DataFieldKey + ",";
            }
            return string.Format("INSERT INTO {0}({1}) VALUES({2})", Table, str2.Trim(','), str1.Trim(','));
        }

        /// <inheritdoc />
        public override List<Exception> ValidateSettings()
        {
            List<Exception> source = [];
            if (string.IsNullOrEmpty(Table))
            {
                Exception exception = new("'Table' setting has not been set");
                source.Add(exception);
            }
            if (string.IsNullOrEmpty(Connection))
            {
                Exception exception = new("'Connection' setting has not been set");
                source.Add(exception);
            }
            if (source.Count != 0)
                return source;
            SqlConnection sqlConnection = new(Connection);
            MsSqlAnalyzer msSqlAnalyzer = new(Connection, _loggerFactory.CreateLogger<MsSqlAnalyzer>());
            try
            {
                try
                {
                    sqlConnection.Open();
                    msSqlAnalyzer.GetTable(Table);
                }
                catch (Exception ex)
                {
                    source.Add(ex);
                }
                finally
                {
                    if (sqlConnection.State == ConnectionState.Open)
                        sqlConnection.Close();
                    msSqlAnalyzer.Dispose();
                    sqlConnection.Dispose();
                }
            }
            catch (Exception ex)
            {
                source.Add(ex);
            }
            return source;
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}