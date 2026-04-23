using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a validation pattern representation.</summary>
    [DataContract(Name = "validationPattern")]
    [Serializable]
    public class ValidationPattern
    {
        /// <summary>Gets or sets the pattern name.</summary>
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the pattern name.</summary>
        [DataMember(Name = "labelKey")]
        public string LabelKey { get; set; } = string.Empty;

        /// <summary>Gets or sets the pattern's regular expression.</summary>
        [DataMember(Name = "pattern")]
        public string Pattern { get; set; } = string.Empty;
    }
}