#if !NET10_0_OR_GREATER
#pragma warning disable CS0618
namespace UmbracoCms.CodeFirst.Helpers
{
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Services;

    [System.Obsolete("Macros are deprecated in Umbraco 14+. Use Yaml2Schema.")]
    public static class MacroHelpers
    {
        public static void CreateMacro(IMacroService macroService, Interfaces.IMacro macro)
        {
            var alreadyCreatedMacro = macroService.GetByAlias(macro.Alias);
            if (alreadyCreatedMacro == null)
            {
                throw new System.NotSupportedException(
                    "Macro creation requires IShortStringHelper in Umbraco 13+. CodeFirst is deprecated; use Yaml2Schema.");
            }
        }
    }
}
#pragma warning restore CS0618
#endif
