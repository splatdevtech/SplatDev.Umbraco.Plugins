using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a result type for a theme.</summary>
    [DataContract(Name = "theme")]
    [Serializable]
    public class Theme
    {
        /// <summary>Gets or sets the theme name.</summary>
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;
    }
}