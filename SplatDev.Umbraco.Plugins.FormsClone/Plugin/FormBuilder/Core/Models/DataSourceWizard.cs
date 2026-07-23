using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a datasource creation wizard.</summary>
    [DataContract(Name = "dataSourceWizard")]
    [Serializable]
    public class DataSourceWizard
    {
        /// <summary>Gets or sets the datasource's key.</summary>
        [DataMember(Name = "dataSourceGuid")]
        public Guid DataSourceGuid { get; set; }

        /// <summary>Gets or sets the associated form name.</summary>
        [DataMember(Name = "formName")]
        public string FormName { get; set; } = string.Empty;

        /// <summary>Gets or sets the field mappings.</summary>
        [DataMember(Name = "mappings")]
        public IEnumerable<DataSourceWizardFieldMapping> Mappings { get; set; } = [];
    }
}