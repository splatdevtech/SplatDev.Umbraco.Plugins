
// Type: Umbraco.Forms.Core.Persistence.Schema.DatabaseColumn
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Schema
{
  public class DatabaseColumn
  {
    public bool IsReadOnly { get; set; }

    public bool IsForeignKey { get; set; }

    public Type Type { get; set; } = typeof (object);

    public int MaxLength { get; set; }

    public bool AllowNulls { get; set; }

    public bool AutoIncrement { get; set; }

    public int NumberScale { get; set; }

    public int NumericPrecision { get; set; }

    public bool IsPrimaryKey { get; set; }

    public string DefaultSetting { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Schema { get; set; } = string.Empty;

    public string PropertyName { get; set; } = string.Empty;

    public string PrimaryKeyTable { get; set; } = string.Empty;

    public string PrimaryKeyColumn { get; set; } = string.Empty;

    public bool IsNumeric => this.Type == typeof (int) || this.Type == typeof (int) || this.Type == typeof (long) || this.Type == typeof (short) || this.Type == typeof (uint) || this.Type == typeof (ushort) || this.Type == typeof (uint) || this.Type == typeof (ulong) || this.Type == typeof (float) || this.Type == typeof (Decimal) || this.Type == typeof (double);

    public bool IsDateTime => this.Type == typeof (DateTime);

    public bool IsString => this.Type == typeof (string);
  }
}
