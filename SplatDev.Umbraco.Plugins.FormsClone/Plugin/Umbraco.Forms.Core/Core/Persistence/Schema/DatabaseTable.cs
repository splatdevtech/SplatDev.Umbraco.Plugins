
// Type: Umbraco.Forms.Core.Persistence.Schema.DatabaseTable
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Schema
{
  public class DatabaseTable
  {
    public DatabaseColumn[] PrimaryKeys => this.Columns.Where<DatabaseColumn>((Func<DatabaseColumn, bool>) (c => c.IsPrimaryKey)).ToArray<DatabaseColumn>();

    public bool HasPrimaryKey => this.PrimaryKey != null;

    public DatabaseColumn? PrimaryKey => this.Columns.FirstOrDefault<DatabaseColumn>((Func<DatabaseColumn, bool>) (c => c.IsPrimaryKey));

    public DatabaseColumn? Descriptor
    {
      get
      {
        DatabaseColumn descriptor = (DatabaseColumn) null;
        foreach (DatabaseColumn column in (IEnumerable<DatabaseColumn>) this.Columns)
        {
          if (!column.IsPrimaryKey && column.IsString & !column.IsForeignKey)
          {
            descriptor = column;
            break;
          }
        }
        if (descriptor == null)
          descriptor = this.PrimaryKey;
        return descriptor;
      }
    }

    public string SchemaName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string FriendlyName { get; set; } = string.Empty;

    public string ClassName { get; set; } = string.Empty;

    public IList<DatabaseColumn> Columns { get; set; } = (IList<DatabaseColumn>) new List<DatabaseColumn>();

    public DatabaseColumn? GetColumn(string columnName) => this.Columns.SingleOrDefault<DatabaseColumn>((Func<DatabaseColumn, bool>) (c => c.Name.Equals(columnName)));
  }
}
