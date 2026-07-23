namespace FormBuilder.Core.Persistence.Schema
{
    public class DatabaseColumn
    {
        public bool IsReadOnly { get; set; }

        public bool IsForeignKey { get; set; }

        public Type Type { get; set; } = typeof(object);

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

        public bool IsNumeric => Type == typeof(int) || Type == typeof(int) || Type == typeof(long) || Type == typeof(short) || Type == typeof(uint) || Type == typeof(ushort) || Type == typeof(uint) || Type == typeof(ulong) || Type == typeof(float) || Type == typeof(decimal) || Type == typeof(double);

        public bool IsDateTime => Type == typeof(DateTime);

        public bool IsString => Type == typeof(string);
    }
}