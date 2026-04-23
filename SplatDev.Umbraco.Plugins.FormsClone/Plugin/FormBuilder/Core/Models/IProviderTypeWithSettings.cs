namespace FormBuilder.Core.Models
{
    /// <summary>Defines a provider type with settings.</summary>
    public interface IProviderTypeWithSettings
    {
        Guid Id { get; }

        IEnumerable<Setting> Settings { get; set; }
    }
}