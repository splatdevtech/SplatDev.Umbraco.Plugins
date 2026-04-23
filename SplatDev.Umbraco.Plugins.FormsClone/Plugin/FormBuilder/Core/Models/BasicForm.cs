using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a basic form representation.</summary>
    [DataContract(Name = "form")]
    [Serializable]
    public class BasicForm
    {
        /// <summary>Gets or sets the form's Id.</summary>
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        /// <summary>Gets or sets the form's name.</summary>
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the form's fields.</summary>
        [DataMember(Name = "fields")]
        public string Fields { get; set; } = string.Empty;

        /// <summary>Gets or sets the form's summary.</summary>
        [DataMember(Name = "summary")]
        public string Summary { get; set; } = string.Empty;
    }
}