namespace SplatDev.Content.Interfaces
{
    public interface ITemplate
    {
        string Alias { get; }
        string FileName { get; }
        string Name { get; }
        string OutputDirectory { get; }
        string ResourceLocation { get; }
    }
}
