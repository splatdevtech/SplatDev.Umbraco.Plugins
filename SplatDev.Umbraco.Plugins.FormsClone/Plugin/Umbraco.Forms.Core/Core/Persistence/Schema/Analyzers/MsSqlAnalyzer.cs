
// Type: Umbraco.Forms.Core.Persistence.Schema.Analyzers.MsSqlAnalyzer
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using System.Data;

using Umbraco.Cms.Core.Scoping;

using IScope = Umbraco.Cms.Infrastructure.Scoping.IScope;
using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

#nullable enable
namespace Umbraco.Forms.Core.Persistence.Schema.Analyzers
{
    public class MsSqlAnalyzer : IDisposable
    {
        private readonly ILogger<MsSqlAnalyzer> _logger;
        private SqlConnection? _conn = new SqlConnection();

        public MsSqlAnalyzer(IScopeProvider scopeProvider, ILogger<MsSqlAnalyzer> logger)
        {
            this._logger = logger;
            using (IScope scope = scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, false))
                this.Init(scope.Database.ConnectionString);
        }

        public MsSqlAnalyzer(string connection, ILogger<MsSqlAnalyzer> logger)
        {
            this._logger = logger;
            this.Init(connection);
        }

        private void Init(string connection) => this._conn = new SqlConnection(connection);

        public DatabaseTable GetTable(string table)
        {
            DatabaseTable table1 = new DatabaseTable()
            {
                Name = table,
                FriendlyName = table,
                Columns = this.GetColumns(table)
            };
            this.SetKeyMap(ref table1);
            return table1;
        }

        private void SetKeyMap(ref DatabaseTable table)
        {
            DataTable sqlPrimaryKeyInfo = this.GetSqlPrimaryKeyInfo(table.Name);
            DataTable sqlForeignKeys = this.GetSqlForeignKeys(table.Name);
            foreach (DataRow row in (InternalDataCollectionBase)sqlPrimaryKeyInfo.Rows)
            {
                string columnName = row["ColumnName"].ToString();
                string str = row["BaseSchemaName"].ToString();
                DatabaseColumn column = table.GetColumn(columnName);
                if (column != null)
                {
                    column.Schema = str;
                    column.IsPrimaryKey = bool.Parse(row["IsKey"].ToString());
                    column.AutoIncrement = bool.Parse(row["IsAutoIncrement"].ToString());
                    column.IsReadOnly = bool.Parse(row["IsReadOnly"].ToString());
                    column.AllowNulls = bool.Parse(row["AllowDBNull"].ToString());
                }
            }
            foreach (DataRow row in (InternalDataCollectionBase)sqlForeignKeys.Rows)
            {
                string columnName = row["FK_COLUMN_NAME"].ToString();
                string str = row["FK_TABLE_SCHEMA"].ToString();
                DatabaseColumn column = table.GetColumn(columnName);
                if (column != null)
                {
                    column.Schema = str;
                    column.IsForeignKey = true;
                    column.PrimaryKeyColumn = row["PK_COLUMN_NAME"].ToString();
                    column.PrimaryKeyTable = row["PK_TABLE_NAME"].ToString();
                }
            }
        }

        private List<DatabaseColumn> GetColumns(string table)
        {
            List<DatabaseColumn> columns = new List<DatabaseColumn>();
            try
            {
                foreach (DataRow row in (InternalDataCollectionBase)this.GetSqlColumns(table).Rows)
                    columns.Add(MsSqlAnalyzer.GetColumnMap(row));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error in GetColumns for Table {TableName}", table);
                throw;
            }
            return columns;
        }

        private static DatabaseColumn GetColumnMap(DataRow dr)
        {
            DatabaseColumn columnMap = new DatabaseColumn()
            {
                Name = dr["COLUMN_NAME"].ToString(),
                Schema = dr["TABLE_SCHEMA"].ToString(),
                Type = MsSqlAnalyzer.SqlToNetTypeConverter(dr["DATA_TYPE"].ToString()),
                MaxLength = -1
            };
            if (!string.IsNullOrEmpty(dr["CHARACTER_MAXIMUM_LENGTH"].ToString()))
                columnMap.MaxLength = int.Parse(dr["CHARACTER_MAXIMUM_LENGTH"].ToString());
            if (!string.IsNullOrEmpty(dr["COLUMN_DEFAULT"].ToString()))
                columnMap.DefaultSetting = dr["COLUMN_DEFAULT"].ToString();
            columnMap.AllowNulls = dr["IS_NULLABLE"].ToString() == "YES";
            return columnMap;
        }

        public DataTable GetSqlTables(string tableName)
        {
            _conn.Open();
            DataTable schema = _conn.GetSchema("Tables", new string[1]
            {
        tableName
            });
            _conn.Close();
            return schema;
        }

        public DataTable GetSqlColumns(string tableName)
        {
            _conn.Open();
            DataTable schema = _conn.GetSchema("Columns", new string[3]
            {
        null,
        null,
        tableName
            });
            _conn.Close();
            return schema;
        }

        private DataTable GetSqlPrimaryKeys(string tableName)
        {
            _conn.Open();
            DataTable schema = _conn.GetSchema("PrimaryKeys", new string[1]
            {
        tableName
            });
            _conn.Close();
            return schema;
        }

        private DataTable GetSqlPrimaryKeyInfo(string tableName)
        {
            _conn.Open();
            DataTable schemaTable = new SqlCommand("SELECT TOP 1 * FROM " + tableName, this._conn).ExecuteReader(CommandBehavior.KeyInfo).GetSchemaTable();
            _conn.Close();
            return schemaTable;
        }

        private DataTable GetSqlForeignKeys(string tableName)
        {
            _conn.Open();
            DataTable schema = _conn.GetSchema("ForeignKeys", new string[1]
            {
        tableName
            });
            _conn.Close();
            return schema;
        }

        private static Type SqlToNetTypeConverter(string sqlTypeName)
        {
            if (sqlTypeName != null)
            {
                switch (sqlTypeName.Length)
                {
                    case 3:
                        switch (sqlTypeName[0])
                        {
                            case 'b':
                                if (sqlTypeName == "bit")
                                    return typeof(bool);
                                goto label_28;
                            case 'i':
                                if (sqlTypeName == "int")
                                    break;
                                goto label_28;
                            default:
                                goto label_28;
                        }
                        break;
                    case 4:
                        switch (sqlTypeName[0])
                        {
                            case 'c':
                                if (sqlTypeName == "char")
                                    goto label_25;
                                else
                                    goto label_28;
                            case 't':
                                if (sqlTypeName == "text")
                                    goto label_25;
                                else
                                    goto label_28;
                            default:
                                goto label_28;
                        }
                    case 5:
                        switch (sqlTypeName[1])
                        {
                            case 'c':
                                if (sqlTypeName == "nchar")
                                    goto label_25;
                                else
                                    goto label_28;
                            case 'l':
                                if (sqlTypeName == "float")
                                    goto label_26;
                                else
                                    goto label_28;
                            case 't':
                                if (sqlTypeName == "ntext")
                                    goto label_25;
                                else
                                    goto label_28;
                            default:
                                goto label_28;
                        }
                    case 7:
                        switch (sqlTypeName[0])
                        {
                            case 'd':
                                if (sqlTypeName == "decimal")
                                    goto label_26;
                                else
                                    goto label_28;
                            case 'n':
                                if (sqlTypeName == "numeric")
                                    goto label_26;
                                else
                                    goto label_28;
                            case 't':
                                if (sqlTypeName == "tinyint")
                                    break;
                                goto label_28;
                            case 'v':
                                if (sqlTypeName == "varchar")
                                    goto label_25;
                                else
                                    goto label_28;
                            default:
                                goto label_28;
                        }
                        break;
                    case 8:
                        switch (sqlTypeName[0])
                        {
                            case 'd':
                                if (sqlTypeName == "datetime")
                                    goto label_27;
                                else
                                    goto label_28;
                            case 'n':
                                if (sqlTypeName == "nvarchar")
                                    goto label_25;
                                else
                                    goto label_28;
                            case 's':
                                if (sqlTypeName == "smallint")
                                    break;
                                goto label_28;
                            default:
                                goto label_28;
                        }
                        break;
                    case 9:
                        if (sqlTypeName == "datetime2")
                            goto label_27;
                        else
                            goto label_28;
                    case 13:
                        if (sqlTypeName == "smalldatetime")
                            goto label_27;
                        else
                            goto label_28;
                    default:
                        goto label_28;
                }
                return typeof(int);
            label_25:
                return typeof(string);
            label_26:
                return typeof(double);
            label_27:
                return typeof(DateTime);
            }
        label_28:
            throw new Exception("DataType Not Supported");
        }

        public void Dispose()
        {
            if (_conn.State == ConnectionState.Open)
                _conn.Close();
            _conn.Dispose();
            this._conn = null;
        }
    }
}
