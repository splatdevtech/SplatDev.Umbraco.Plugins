namespace UmbracoCms.CodeFirst.Helpers
{
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Services;

    public static class MacroHelpers
    {
        public static void CreateMacro(IMacroService macroService, Interfaces.IMacro macro)
        {
            var alreadyCreatedMacro = macroService.GetByAlias("aliasName");
            if (alreadyCreatedMacro == null)
            {
                var newMacro = new Macro(macro.Alias, macro.Name, macro.MacroSource, macro.MacroType, macro.CacheByPage, macro.CacheByMember, macro.DontRender, macro.UseInEditor, macro.CacheDuration);
                macroService.Save(newMacro);
            }
        }
    }
}
