using FormBuilder.Core.Interfaces;

namespace FormBuilder.Core.Providers.Themes
{
    /// <summary>Defines the default theme and files.</summary>
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