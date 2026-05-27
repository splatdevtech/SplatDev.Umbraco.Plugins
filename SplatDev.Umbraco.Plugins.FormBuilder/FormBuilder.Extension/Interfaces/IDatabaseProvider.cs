using NPoco;

namespace FormBuilder.Extension.Interfaces
{
    public interface IDatabaseProvider
    {
        string ProviderType { get; }
        void Configure(DatabaseFactory factory);
    }
}
