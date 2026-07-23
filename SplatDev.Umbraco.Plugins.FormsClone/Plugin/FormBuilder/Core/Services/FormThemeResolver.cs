using FormBuilder.Core.Configuration;
using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;

using Microsoft.Extensions.Options;

using System.Diagnostics.CodeAnalysis;

using Umbraco.Cms.Core.IO;
using Umbraco.Extensions;

namespace FormBuilder.Core.Services
{
    /// <summary>Resolves theme specific partial views for forms.</summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    internal sealed class FormThemeResolver(
      IFileSystem partialViewFileSystem,
      IOptions<FormDesignSettings> config,
      ThemeCollection themeCollection) : IFormThemeResolver
    {
        private readonly IFileSystem _partialViewFileSystem = partialViewFileSystem;
        private readonly FormDesignSettings _config = config.Value;
        private readonly ThemeCollection _themeCollection = themeCollection;

        /// <summary>
        /// Retrieve the view to show the forms submitted state, Default is /Vews/Partials/Forms/Themes/default/Submitted.cshtml
        /// Method will search for a theme specific view at /Views/Partials/Forms/Themes/[themealias]/Submitted.cshtml
        /// </summary>
        /// <returns>Path to the view</returns>
        public string? GetFormSubmittedView(FormViewModel form) => GetThemedView("Submitted", form.FormId, form.Theme);

        /// <summary>
        /// Retrieve the view to render the form fieldsets and fields, Default is /Vews/Partials/Forms/Themes/default/Form.cshtml
        /// Method will search for a theme specific view at /Views/Partials/Forms/Themes/[themealias]/Form.cshtml
        /// </summary>
        /// <returns>Path to the view</returns>
        public string? GetFormView(FormViewModel form) => GetThemedView(form.SubmitHandled ? "Submitted" : "Form", form.FormId, form.Theme);

        /// <summary>
        /// Retrieve the view to render the entire form and validation logic, Default is /Vews/Partials/Forms/Themes/default/Render.cshtml
        /// Method will search for a theme specific view at /Views/Partials/Forms/Themes/[themealias]/Render.cshtml
        /// </summary>
        /// <returns>Path to the view</returns>
        /// <remarks>Default is /Views/Partials/Forms/Render.cshtml</remarks>
        public string? GetFormRender(FormViewModel form) => GetThemedView("Render", form.FormId, form.Theme);

        /// <summary>
        /// Retrieve the view to render the form javascript code, Default is /Vews/Partials/Forms/Themes/default/Script.cshtml
        /// Method will search for a theme specific view at /Views/Partials/Forms/Themes/[themealias]/Script.cshtml
        /// </summary>
        /// <returns>Path to the view</returns>
        public string? GetScriptView(FormViewModel form) => GetThemedView("Script", form.FormId, form.Theme);

        /// <summary>
        /// Retrieve the view to render the multi-page form summary.
        /// </summary>
        /// <returns>Path to the view</returns>
        public string? GetMultiPageFormPagingDetailsView(FormViewModel form) => GetThemedView("MultiPageFormPagingDetails", form.FormId, form.Theme);

        /// <summary>
        /// Retrieve the view to render the multi-page form summary.
        /// </summary>
        /// <returns>Path to the view</returns>
        public string? GetMultiPageFormSummaryView(FormViewModel form) => GetThemedView("MultiPageFormSummary", form.FormId, form.Theme);

        /// <summary>
        /// Retrieve the view to render the generic read-only field view.
        /// </summary>
        /// <returns>Path to the view</returns>
        public string? GetGenericReadOnlyFieldView(FormViewModel form) => GetThemedView("Fieldtypes/ReadOnly", form.FormId, form.Theme);

        /// <summary>
        /// Retrieve the view to render the form date picker code, Default is /Vews/Partials/Forms/Themes/default/DatePicker.cshtml
        /// Method will search for a theme specific view at /Views/Partials/Forms/Themes/[themealias]/DatePicker.cshtml
        /// </summary>
        /// <returns>Path to the view</returns>
        public string? GetDatePicker(string? theme) => GetThemedView("DatePicker", theme);

        /// <summary>
        /// Retrieve the view to render a specific fieldtype, Default is /Vews/Partials/Forms/Themes/default/Fieldtypes/FieldType.[FieldTypeAlias].cshtml
        /// Method will search for a theme specific view at /Views/Partials/Forms/Themes/[themealias]/Fieldtypes/FieldType.[FieldTypeAlias].cshtml
        /// </summary>
        /// <returns>Path to the view</returns>
        public string? GetFieldView(FormViewModel form, FieldViewModel field)
        {
            if (field.FieldType is null)
                throw new InvalidOperationException("Cannot get view for field " + field.Name + " as field type is not specified.");
            string fieldTypeViewName = field.FieldType.FieldTypeViewName;
            if (fieldTypeViewName.Contains('/'))
                return fieldTypeViewName;
            string path = string.Format("{0}/{1}/Fieldtypes/{2}", "Forms", form.FormId.ToString().ToLowerInvariant(), fieldTypeViewName);
            if (_partialViewFileSystem.FileExists(path))
                return _partialViewFileSystem.GetUrl(path);
            return !string.IsNullOrEmpty(form.Theme) && TryGetPathToView(form.Theme, fieldTypeViewName, "Fieldtypes/", out string? verifiedPath) || TryGetPathToView(GetDefaultTheme(), fieldTypeViewName, "Fieldtypes/", out verifiedPath) ? verifiedPath : _partialViewFileSystem.GetUrl(string.Format("{0}/default/Fieldtypes/{1}", "Forms/Themes", fieldTypeViewName));
        }

        /// <inheritdoc />
        public string? GetReadOnlyFieldView(FormViewModel form, FieldViewModel field) => GetFieldView(form, field)?.Replace(".cshtml", ".ReadOnly.cshtml");

        private string? GetThemedView(string view, string? theme) => GetThemedView(view, Guid.Empty, theme);

        private string? GetThemedView(string view, Guid formId, string? theme)
        {
            if (formId != Guid.Empty)
            {
                string path = string.Format("{0}/{1}/{2}.cshtml", "Forms", formId.ToString().ToLowerInvariant(), view);
                if (_partialViewFileSystem.FileExists(path))
                    return _partialViewFileSystem.GetUrl(path);
            }
            return !string.IsNullOrEmpty(theme) && TryGetPathToView(theme, view, string.Empty, out string? verifiedPath) || TryGetPathToView(GetDefaultTheme(), string.Empty, view, out verifiedPath) ? verifiedPath : string.Format("{0}/{1}/{2}.cshtml", "/Views/Partials/Forms/Themes", "default", view);
        }

        private bool TryGetPathToView(
            string theme,
            string view,
            string themeFolder,
            [NotNullWhen(true)] out string? verifiedPath)
        {
            string compareTo = string.Format("{0}/{1}/{2}{3}",
                "/Views/Partials/Forms/Themes",
                theme,
                themeFolder,
                view.EndsWith(".cshtml") ? view : view + ".cshtml");

            ITheme? theme1 = _themeCollection.FirstOrDefault(x => x.Name == theme);
            if (theme1 != null && theme1.Files.InvariantContains(compareTo))
            {
                verifiedPath = compareTo;
                return true;
            }

            string path = string.Format("{0}/{1}/{2}{3}",
                "Forms/Themes",
                theme,
                themeFolder,
                view.EndsWith(".cshtml") ? view : view + ".cshtml");

            if (_partialViewFileSystem.FileExists(path))
            {
                verifiedPath = _partialViewFileSystem.GetUrl(path);
                return true;
            }

            verifiedPath = null;
            return false;
        }

        private string GetDefaultTheme()
        {
            string defaultTheme = _config.DefaultTheme;
            if (string.IsNullOrWhiteSpace(defaultTheme))
                defaultTheme = "default";
            return defaultTheme;
        }
    }
}