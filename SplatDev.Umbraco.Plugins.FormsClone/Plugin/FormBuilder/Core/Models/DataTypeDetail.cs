namespace FormBuilder.Core.Models
{
    /// <summary>
    /// Represents a data type when used in the backoffice for Forms.
    /// </summary>
    /// <remarks>
    /// Use case for this is exposing which data type is configured for rich text editing in Forms.
    /// </remarks>
    public class DataTypeDetail
    {
        /// <summary>Gets or sets the data type Id.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the data type key.</summary>
        public Guid Key { get; set; }

        /// <summary>Gets or sets the data type name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the configuration data</summary>
        public IDictionary<string, object> ConfigurationData { get; set; } = new Dictionary<string, object>();
    }
}