
// Type: Umbraco.Forms.Core.Providers.DatasourceTypes.MsSql
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using System.Data;
using System.Data.Common;
using System.Text;

using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Interfaces;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Persistence.Schema;
using Umbraco.Forms.Core.Persistence.Schema.Analyzers;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Providers.DatasourceTypes
{
    public class MsSql : FormDataSourceType, IFieldPreValueCrudSupport
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

        public MsSql(
          IFormService formService,
          IFieldTypeStorage fieldTypeStorage,
          ILoggerFactory loggerFactory)
        {
            this._formService = formService;
            this._fieldTypeStorage = fieldTypeStorage;
            this._loggerFactory = loggerFactory;
            this._logger = loggerFactory.CreateLogger<MsSql>();
            this.Id = new Guid("F19506F3-EFEA-4B13-A308-89348F69DF91");
            this.Name = "SQL Database";
            this.Alias = "sqlDatabase";
            this.Description = "Connects to any SQL Server database table and constructs a datasource from it";
            this.Icon = "icon-server-alt";
        }

        [Setting("Connection String", Description = "SQL Server connection string", DisplayOrder = 10, IsMandatory = true, View = "Umb.PropertyEditorUi.TextArea")]
        public virtual string Connection { get; set; } = string.Empty;

        [Setting("Table Name", Description = "Table name", DisplayOrder = 20, IsMandatory = true)]
        public virtual string Table { get; set; } = string.Empty;

        public override List<Record> GetRecords(
          Form form,
          int page,
          int maxItems,
          object sortByField,
          RecordSorting order)
        {
            List<Record> records = new List<Record>();
            List<FormDataSourceMapping> fieldMappings = form.DataSource?.Mappings ?? new List<FormDataSourceMapping>();
            SqlConnection sqlConnection = new SqlConnection(this.Connection);
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = this.PagingSql(page, maxItems, sortByField, order, fieldMappings);
            sqlConnection.Open();
            SqlDataReader dr = command.ExecuteReader();
            while (dr != null && dr.Read())
                records.Add(MsSql.DataReaderToRecord(dr, form));
            sqlConnection.Close();
            ((DbDataReader)dr)?.Dispose();
            command.Dispose();
            return records;
        }

        private static Record DataReaderToRecord(SqlDataReader dr, Form form)
        {
            Record record = new Record()
            {
                Created = DateTime.Now,
                Updated = DateTime.Now,
                Form = form.Id,
                State = FormState.Submitted
            };
            RecordField recordField = new RecordField();
            foreach (Umbraco.Forms.Core.Models.Field allField in form.AllFields)
            {
                if (allField.DataSourceFieldKey != null)
                {
                    FormDataSourceMapping mapping = form.DataSource?.GetMapping(allField.DataSourceFieldKey);
                    if (mapping != null)
                    {
                        string prevalueValueField = allField.DataSourceFieldKey.ToString();
                        if (!string.IsNullOrEmpty(mapping.PrevalueValueField))
                            prevalueValueField = mapping.PrevalueValueField;
                        if (((DbDataReader)dr)[prevalueValueField] != null)
                        {
                            recordField.Field = allField;
                            recordField.Values.Add(((DbDataReader)dr)[prevalueValueField]);
                            recordField.Key = new Guid();
                            record.RecordFields.Add(allField.Id, recordField);
                        }
                    }
                }
            }
            return record;
        }

        public override Record InsertRecord(Record record)
        {
            List<FormDataSourceMapping> mappings = this._formService.Get(record.Form)?.DataSource?.Mappings ?? new List<FormDataSourceMapping>();
            string str = this.InsertSql(mappings);
            SqlConnection sqlConnection = new SqlConnection(this.Connection);
            SqlCommand command = sqlConnection.CreateCommand();
            try
            {
                command.CommandText = str;
                foreach (FormDataSourceMapping dataSourceMapping in mappings)
                {
                    object obj = new object();
                    if (string.IsNullOrEmpty(dataSourceMapping.DefaultValue))
                    {
                        foreach (RecordField recordField in record.RecordFields.Values)
                        {
                            if (recordField.Field?.DataSourceFieldKey == dataSourceMapping.DataFieldKey)
                            {
                                FieldType fieldTypeByField = this._fieldTypeStorage.GetFieldTypeByField(recordField.Field);
                                if (fieldTypeByField != null)
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
                this._logger.LogError(ex, "There was a problem trying to insert record with UniqueID {RecordId} into SQL DataSource Provider Connection for Table: {TableName}", record.UniqueId, Table);
                throw;
            }
            finally
            {
                sqlConnection.Close();
                command.Dispose();
            }
            return record;
        }

        public override Dictionary<object, FormDataSourceField> GetMappedFields() => new Dictionary<object, FormDataSourceField>();

        public override Dictionary<object, FormDataSourceField> GetAvailableFields()
        {
            MsSqlAnalyzer msSqlAnalyzer = new MsSqlAnalyzer(this.Connection, this._loggerFactory.CreateLogger<MsSqlAnalyzer>());
            DatabaseTable table = msSqlAnalyzer.GetTable(this.Table);
            Dictionary<object, FormDataSourceField> availableFields = new Dictionary<object, FormDataSourceField>();
            foreach (DatabaseColumn column in (IEnumerable<DatabaseColumn>)table.Columns)
            {
                FormDataSourceField formDataSourceField = new FormDataSourceField()
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
                    formDataSourceField.AvailablePreValueValueFields = new List<string>();
                    foreach (DataRow row in (InternalDataCollectionBase)msSqlAnalyzer.GetSqlColumns(column.PrimaryKeyTable).Rows)
                    {
                        string str = row["COLUMN_NAME"].ToString();
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
                if (column.DefaultSetting != null)
                    formDataSourceField.IsMandatory = false;
                availableFields.Add(column.Name, formDataSourceField);
            }
            msSqlAnalyzer.Dispose();
            return availableFields;
        }

        public override Dictionary<object, string> GetPreValues(Umbraco.Forms.Core.Models.Field field, Form form)
        {
            Dictionary<object, string> preValues = new Dictionary<object, string>();
            if (field.DataSourceFieldKey != null)
            {
                FormDataSourceMapping dataSourceMapping = (form.DataSource?.Mappings ?? new List<FormDataSourceMapping>()).FirstOrDefault<FormDataSourceMapping>(x => string.Equals(x.DataFieldKey.ToString(), field.DataSourceFieldKey.ToString(), StringComparison.CurrentCultureIgnoreCase));
                if (dataSourceMapping == null)
                    return preValues;
                SqlConnection sqlConnection = new SqlConnection(this.Connection);
                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandText = string.Format("SELECT [{1}] AS [val], [{2}] AS [key] FROM {0} ORDER BY {1} ASC", dataSourceMapping.PrevalueTable, dataSourceMapping.PrevalueValueField, dataSourceMapping.PrevalueKeyfield);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = command.ExecuteReader();
                while (sqlDataReader != null && sqlDataReader.Read())
                    preValues.Add(((DbDataReader)sqlDataReader)["key"], ((DbDataReader)sqlDataReader)["val"].ToString());
                sqlConnection.Close();
                ((DbDataReader)sqlDataReader)?.Dispose();
                sqlConnection.Dispose();
                command.Dispose();
            }
            return preValues;
        }

        public PreValue InsertValue(PreValue preValue, Umbraco.Forms.Core.Models.Field field, Form form)
        {
            switch (field?.DataSourceFieldKey)
            {
                case null:
                    return preValue;
                default:
                    FormDataSourceMapping dataSourceMapping = (form.DataSource?.Mappings ?? new List<FormDataSourceMapping>()).FirstOrDefault<FormDataSourceMapping>(x => x.DataFieldKey == field.DataSourceFieldKey);
                    if (dataSourceMapping == null)
                        return preValue;
                    SqlConnection sqlConnection = new SqlConnection(this.Connection);
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
                        this._logger.LogError(ex, "There was a problem trying to insert a PreValue with value {PreValue} for Field: {FieldName} in Form {FormName} with an id {FormId} into SQL DataSource Provider for Table: {TableName}", preValue.Value, field.Caption, form.Name, form.Id, Table);
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

        public PreValue UpdateValue(PreValue preValue, Umbraco.Forms.Core.Models.Field field, Form form)
        {
            if (field != null && field.DataSourceFieldKey != null)
            {
                FormDataSourceMapping dataSourceMapping = (form.DataSource?.Mappings ?? new List<FormDataSourceMapping>()).FirstOrDefault<FormDataSourceMapping>(x => x.DataFieldKey == field.DataSourceFieldKey);
                if (dataSourceMapping == null)
                    return preValue;
                SqlConnection sqlConnection = new SqlConnection(this.Connection);
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
                    this._logger.LogError(ex, "There was a problem trying to update a PreValue with value {PreValue} for Field: {FieldName} in Form {FormName} with an id {FormId} into SQL DataSource Provider for Table: {TableName}", preValue.Value, field.Caption, form.Name, form.Id, Table);
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

        public bool DeleteValue(PreValue preValue, Umbraco.Forms.Core.Models.Field field, Form form)
        {
            switch (field?.DataSourceFieldKey)
            {
                case null:
                    return true;
                default:
                    FormDataSourceMapping dataSourceMapping = (form.DataSource?.Mappings ?? new List<FormDataSourceMapping>()).FirstOrDefault<FormDataSourceMapping>(x => x.DataFieldKey == field.DataSourceFieldKey);
                    if (dataSourceMapping == null)
                        return true;
                    SqlConnection sqlConnection = new SqlConnection(this.Connection);
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
                        this._logger.LogError(ex, "There was a problem trying to delete a PreValue with value {PreValue} for Field: {FieldName} in Form {FormName} with an id {FormId} into SQL DataSource Provider for Table: {TableName}", preValue.Value, field.Caption, form.Name, form.Id, Table);
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

        public void SortValues(Guid fieldId, List<PreValue> values, Umbraco.Forms.Core.Models.Field field, Form form)
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

        public override List<Exception> ValidateSettings()
        {
            List<Exception> source = new List<Exception>();
            if (string.IsNullOrEmpty(this.Table))
            {
                Exception exception = new Exception("'Table' setting has not been set");
                source.Add(exception);
            }
            if (string.IsNullOrEmpty(this.Connection))
            {
                Exception exception = new Exception("'Connection' setting has not been set");
                source.Add(exception);
            }
            if (source.Any<Exception>())
                return source;
            SqlConnection sqlConnection = new SqlConnection(this.Connection);
            MsSqlAnalyzer msSqlAnalyzer = new MsSqlAnalyzer(this.Connection, this._loggerFactory.CreateLogger<MsSqlAnalyzer>());
            try
            {
                try
                {
                    sqlConnection.Open();
                    msSqlAnalyzer.GetTable(this.Table);
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
