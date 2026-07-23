namespace SplatDev.Database.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class DatabaseMigration
    {
      //  protected readonly string TableName;
      //  private const string tbname = "tigerbayVillas";
      //  public List<MigratedColumn> AlteredColumns { get; set; }
      //  public List<MigratedColumn> AddedColumns { get; set; }
      //  public DatabaseContext DatabaseContext { get; set; }
      //  public DatabaseMigration(Database db, ISqlSyntaxProvider sqlSyntax, ILogger logger, string tableName)
      //: base(sqlSyntax, logger)
      //  {
      //      TableName = tableName;
      //  }
      //  public override void Up()
      //  {
      //      foreach (var addedColumn in AddedColumns)
      //      {
      //          string dataType = string.Empty;
      //          if (addedColumn.DataType == typeof(int))
      //              if (addedColumn.Nullable)
      //                  Alter.Table(addedColumn.TableName).AddColumn(addedColumn.ColumnName).AsInt16().Nullable();
      //              else Alter.Table(addedColumn.TableName).AddColumn(addedColumn.ColumnName).AsInt16().NotNullable();

      //          if (addedColumn.DataType == typeof(Int32))
      //              if (addedColumn.Nullable)
      //                  Alter.Table(addedColumn.TableName).AddColumn(addedColumn.ColumnName).AsInt32().Nullable();
      //              else Alter.Table(addedColumn.TableName).AddColumn(addedColumn.ColumnName).AsInt32().NotNullable();

      //          if (addedColumn.DataType == typeof(Int64))
      //              if (addedColumn.Nullable)
      //                  Alter.Table(addedColumn.TableName).AddColumn(addedColumn.ColumnName).AsInt64().Nullable();
      //              else Alter.Table(addedColumn.TableName).AddColumn(addedColumn.ColumnName).AsInt64().NotNullable();

      //          if (addedColumn.DataType == typeof(bool))
      //              if (addedColumn.Nullable)
      //                  Alter.Table(addedColumn.TableName).AddColumn(addedColumn.ColumnName).AsBoolean().Nullable();
      //              else Alter.Table(addedColumn.TableName).AddColumn(addedColumn.ColumnName).AsBoolean().NotNullable();

      //          if (addedColumn.DataType == typeof(decimal))
      //              if (addedColumn.Nullable)
      //                  Alter.Table(addedColumn.TableName).AddColumn(addedColumn.ColumnName).AsDecimal().Nullable();
      //              else Alter.Table(addedColumn.TableName).AddColumn(addedColumn.ColumnName).AsDecimal().NotNullable();

      //          if (addedColumn.DataType == typeof(string))
      //              if (addedColumn.Nullable)
      //                  Alter.Table(addedColumn.TableName).AddColumn(addedColumn.ColumnName).AsString().Nullable();
      //              else Alter.Table(addedColumn.TableName).AddColumn(addedColumn.ColumnName).AsString().NotNullable();

      //          if (addedColumn.DataType == typeof(float))
      //              if (addedColumn.Nullable)
      //                  Alter.Table(addedColumn.TableName).AddColumn(addedColumn.ColumnName).AsFloat().Nullable();
      //              else Alter.Table(addedColumn.TableName).AddColumn(addedColumn.ColumnName).AsFloat().NotNullable();

      //          if (addedColumn.DataType == typeof(DateTime))
      //              if (addedColumn.Nullable)
      //                  Alter.Table(addedColumn.TableName).AddColumn(addedColumn.ColumnName).AsDateTime().Nullable();
      //              else Alter.Table(addedColumn.TableName).AddColumn(addedColumn.ColumnName).AsDateTime().NotNullable();

      //          try
      //          {
      //              DatabaseContext.Database.Execute($"ALTER TABLE [{addedColumn.TableName}] ALTER COLUMN [{addedColumn.ColumnName}] NVARCHAR(MAX)");

      //          }
      //          catch (System.Data.SqlServerCe.SqlCeException)
      //          {
      //              //if using SQLCE cannot go past 4000 characters
      //              DatabaseContext.Database.Execute($"ALTER TABLE [{addedColumn.TableName}] ALTER COLUMN [{addedColumn.ColumnName}] NVARCHAR(4000)");

      //          }
      //      }
      //  }

      //  public override void Down()
      //  {
      //      throw new NotImplementedException();
      //  }
      //  protected bool ColumnExists(string tableName, string columnName)
      //  {

      //      var columns = SqlSyntax.GetColumnsInSchema(_database);
      //      var doesColumnExist =
      //          columns.Any(
      //              x =>
      //                  string.Equals(x.TableName, tableName) &&
      //                  string.Equals(x.ColumnName, columnName)
      //          );

      //      return doesColumnExist;
      //  }

    }
    public class MigratedColumn
    {
        public string ColumnName { get; set; }
        public string TableName { get; set; }
        public Type DataType { get; set; }
        public bool Nullable { get; set; }
        public bool NvarcharMax { get; set; }
        public PropertyInfo Property { get; set; }
    }
}
