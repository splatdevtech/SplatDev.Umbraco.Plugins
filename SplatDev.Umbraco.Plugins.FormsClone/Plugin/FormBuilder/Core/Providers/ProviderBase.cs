using System.Runtime.Serialization;

namespace FormBuilder.Core.Providers
{
    [DataContract(Name = "providerBase")]
    [Serializable]
    public abstract class ProviderBase
    {
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "alias")]
        public string Alias { get; set; } = string.Empty;

        [DataMember(Name = "description")]
        public string Description { get; set; } = string.Empty;

        [DataMember(Name = "icon")]
        public string Icon { get; set; } = string.Empty;

        [DataMember(Name = "group")]
        public string Group { get; set; } = string.Empty;
    }
}