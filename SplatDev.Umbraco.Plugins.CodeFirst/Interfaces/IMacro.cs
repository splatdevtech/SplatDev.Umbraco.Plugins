namespace SplatDev.Umbraco.Plugins.CodeFirst.Interfaces
{
    using Umbraco.Cms.Core.Models;

    public interface IMacro
    {
        string Alias { get; }
        bool CacheByMember { get; }
        bool CacheByPage { get; }
        int CacheDuration { get; }
        string ControlAssembly { get; }
        string ControlType { get; }
        bool DontRender { get; }
        string MacroSource { get; }
        MacroTypes MacroType { get; }
        string Name { get; }
        bool UseInEditor { get; }
    }
}
