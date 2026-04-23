namespace FormBuilder.Core.Persistence.Schema
{
    public class DatabaseTable
    {
        public DatabaseColumn[]? PrimaryKeys => [.. Columns?.Where(c => c.IsPrimaryKey) ?? null!];

        public bool HasPrimaryKey => PrimaryKey is not null;

        public DatabaseColumn? PrimaryKey => Columns?.FirstOrDefault(c => c.IsPrimaryKey);

        public DatabaseColumn? Descriptor
        {
            get
            {
                DatabaseColumn? descriptor = null;
                if (Columns is null) return null;
                foreach (DatabaseColumn column in Columns)
                {
                    if (!column.IsPrimaryKey && column.IsString & !column.IsForeignKey)
                    {
                        descriptor = column;
                        break;
                    }
                }
                descriptor ??= PrimaryKey;
                return descriptor;
            }
        }

        public string SchemaName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string FriendlyName { get; set; } = string.Empty;

        public string ClassName { get; set; } = string.Empty;

        public IList<DatabaseColumn>? Columns { get; set; } = [];

        public DatabaseColumn? GetColumn(string columnName) => Columns?.SingleOrDefault(c => c.Name.Equals(columnName));
    }
}