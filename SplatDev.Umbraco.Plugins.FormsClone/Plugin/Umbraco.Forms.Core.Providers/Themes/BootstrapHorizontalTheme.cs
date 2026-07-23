
// Type: Umbraco.Forms.Core.Providers.Themes.BootstrapHorizontalTheme
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Providers.Themes
{
    public sealed class BootstrapHorizontalTheme : BaseTheme, ITheme
    {
        private static readonly string[] _templates =
        [
            "Form",
            "Fieldtypes/FieldType.RadioButtonList",
            "Fieldtypes/FieldType.CheckBoxList"
        ];

        /// <inheritdoc />
        public string Name => "bootstrap3-horizontal";

        /// <inheritdoc />
        public IEnumerable<string> Files => GetTemplatePaths();

        private IEnumerable<string> GetTemplatePaths()
        {
            const string basePath = "/Views/Partials/Forms/Themes";
            foreach (var template in _templates)
            {
                yield return $"{basePath}/{Name}/{template}.cshtml";
            }
        }
    }
}
