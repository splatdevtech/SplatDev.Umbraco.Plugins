using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a setting with a value.</summary>
    [DataContract(Name = "setting")]
    [Serializable]
    public class SettingWithValue
    {
        /// <summary>Gets or sets the setting's name.</summary>
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the setting's alias.</summary>
        [DataMember(Name = "alias")]
        public string Alias { get; set; } = string.Empty;

        /// <summary>Gets or sets the setting's description.</summary>
        [DataMember(Name = "description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>Gets or sets the setting's prevalues.</summary>
        [DataMember(Name = "prevalues")]
        public IEnumerable<string> Prevalues { get; set; } = [];

        /// <summary>Gets or sets the setting's view.</summary>
        [DataMember(Name = "view")]
        public string View { get; set; } = string.Empty;

        /// <summary>Gets or sets the setting's value.</summary>
        [DataMember(Name = "value")]
        public string Value { get; set; } = string.Empty;

        /// <summary>Gets or sets the setting's display order.</summary>
        [DataMember(Name = "displayOrder")]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the workflow setting should be readonly.
        /// </summary>
        [DataMember(Name = "isReadOnly")]
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the workflow setting is mandatory for completion.
        /// </summary>
        [DataMember(Name = "isMandatory")]
        public bool IsMandatory { get; set; }
    }
}