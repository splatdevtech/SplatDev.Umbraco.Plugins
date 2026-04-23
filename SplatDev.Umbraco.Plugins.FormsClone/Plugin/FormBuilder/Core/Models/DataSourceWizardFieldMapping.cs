using FormBuilder.Core.Enums;

using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>
    /// Defines a datasource creation wizard's field mapping entry.
    /// </summary>
    [DataContract(Name = "dataSourceWizardFieldMapping")]
    [Serializable]
    public class DataSourceWizardFieldMapping
    {
        /// <summary>Gets or sets the key.</summary>
        [DataMember(Name = "key")]
        public string Key { get; set; } = string.Empty;

        /// <summary>Gets or sets the field's name.</summary>
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the field should be included.
        /// </summary>
        [DataMember(Name = "include")]
        public bool Include { get; set; }

        /// <summary>Gets or sets the primary key field.</summary>
        [DataMember(Name = "prevalueKeyField")]
        public string PrevalueKeyField { get; set; } = string.Empty;

        /// <summary>Gets or sets the prevalue value field.</summary>
        [DataMember(Name = "prevalueValueField")]
        public string PrevalueValueField { get; set; } = string.Empty;

        /// <summary>Gets or sets the prevalue source.</summary>
        [DataMember(Name = "prevalueSource")]
        public string PrevalueSource { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of available prevalue value fields.
        /// </summary>
        [DataMember(Name = "availablePrevalueValueFields")]
        public List<string> AvailablePrevalueValueFields { get; set; } = [];

        /// <summary>
        /// Gets or sets a value indicating whether the field is a foreign key.
        /// </summary>
        [DataMember(Name = "isForeignKey")]
        public bool IsForeignKey { get; set; }

        /// <summary>
        /// Gets or sets the a value indicating whether the field is mandatory.
        /// </summary>
        [DataMember(Name = "isMandatory")]
        public bool IsMandatory { get; set; }

        /// <summary>Gets or sets the field's datatype.</summary>
        [DataMember(Name = "dataType")]
        public FieldDataType DataType { get; set; }

        /// <summary>Gets or sets the field's default value.</summary>
        [DataMember(Name = "defaultValue")]
        public string DefaultValue { get; set; } = string.Empty;

        /// <summary>Gets or sets the field type Id.</summary>
        [DataMember(Name = "fieldTypeId")]
        public Guid FieldTypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field is a primary key.
        /// </summary>
        [DataMember(Name = "isPrimaryKey")]
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// Gets or sets the a value indicating whether the field allows null values.
        /// </summary>
        [DataMember(Name = "allowNulls")]
        public bool AllowNulls { get; set; }
    }
}