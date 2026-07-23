using FormBuilder.Core.Interfaces;

namespace FormBuilder.Core.Providers.Themes
{
    /// <summary>Defines the default theme and files.</summary>
    public class DefaultTheme : BaseTheme, ITheme
    {
        private const string BaseThemePath = "/Views/Partials/Forms/Themes";

        private static readonly string[] TemplateNames =
        [
            "DatePicker",
            "Form",
            "Render",
            "Script",
            "ScrollToFormScript",
            "Submitted",
            "Fieldtypes/FieldType.CheckBox",
            "Fieldtypes/FieldType.CheckBoxList",
            "Fieldtypes/FieldType.DataConsent",
            "Fieldtypes/FieldType.DatePicker",
            "Fieldtypes/FieldType.DropDownList",
            "Fieldtypes/FieldType.FileUpload",
            "Fieldtypes/FieldType.HiddenField",
            "Fieldtypes/FieldType.PasswordField",
            "Fieldtypes/FieldType.RadioButtonList",
            "Fieldtypes/FieldType.Recaptcha2",
            "Fieldtypes/FieldType.Recaptcha3",
            "Fieldtypes/FieldType.RichText",
            "Fieldtypes/FieldType.Text",
            "Fieldtypes/FieldType.Textarea",
            "Fieldtypes/FieldType.Textfield"
        ];

        /// <inheritdoc />
        public string Name => "default";

        /// <inheritdoc />
        public IEnumerable<string> Files =>
            TemplateNames.Select(t => $"{BaseThemePath}/{Name}/{t}.cshtml");
    }
}