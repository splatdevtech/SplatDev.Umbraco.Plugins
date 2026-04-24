
// Type: Umbraco.Forms.Web.Services.FormThemeResolver
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.Extensions.Options;

using System.Diagnostics.CodeAnalysis;

using Umbraco.Cms.Core.IO;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Interfaces;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Web.Models;


#nullable enable
namespace Umbraco.Forms.Web.Services
{
    internal sealed class FormThemeResolver : IFormThemeResolver
    {
        private readonly IFileSystem _partialViewFileSystem;
        private readonly FormDesignSettings _config;
        private readonly ThemeCollection _themeCollection;

        public FormThemeResolver(
          IFileSystem partialViewFileSystem,
          IOptions<FormDesignSettings> config,
          ThemeCollection themeCollection)
        {
            this._partialViewFileSystem = partialViewFileSystem;
            this._config = config.Value;
            this._themeCollection = themeCollection;
        }

        public string GetFormSubmittedView(FormViewModel form) => this.GetThemedView("Submitted", form.FormId, form.Theme);

        public string GetFormView(FormViewModel form) => this.GetThemedView(form.SubmitHandled ? "Submitted" : "Form", form.FormId, form.Theme);

        public string GetFormRender(FormViewModel form) => this.GetThemedView("Render", form.FormId, form.Theme);

        public string GetScriptView(FormViewModel form) => this.GetThemedView("Script", form.FormId, form.Theme);

        public string GetMultiPageFormPagingDetailsView(FormViewModel form) => this.GetThemedView("MultiPageFormPagingDetails", form.FormId, form.Theme);

        public string GetMultiPageFormSummaryView(FormViewModel form) => this.GetThemedView("MultiPageFormSummary", form.FormId, form.Theme);

        public string GetGenericReadOnlyFieldView(FormViewModel form) => this.GetThemedView("Fieldtypes/ReadOnly", form.FormId, form.Theme);

        public string GetDatePicker(string? theme) => this.GetThemedView("DatePicker", theme);

        public string GetFieldView(FormViewModel form, FieldViewModel field)
        {
            if (field.FieldType == null)
                throw new InvalidOperationException("Cannot get view for field " + field.Name + " as field type is not specified.");
            string fieldTypeViewName = field.FieldType.FieldTypeViewName;
            if (fieldTypeViewName.Contains("/"))
                return fieldTypeViewName;
            string path = string.Format("{0}/{1}/Fieldtypes/{2}", "Forms", form.FormId.ToString().ToLowerInvariant(), fieldTypeViewName);
            if (this._partialViewFileSystem.FileExists(path))
                return this._partialViewFileSystem.GetUrl(path);
            string verifiedPath;
            return !string.IsNullOrEmpty(form.Theme) && this.TryGetPathToView(form.Theme, fieldTypeViewName, "Fieldtypes/", out verifiedPath) || this.TryGetPathToView(this.GetDefaultTheme(), fieldTypeViewName, "Fieldtypes/", out verifiedPath) ? verifiedPath : this._partialViewFileSystem.GetUrl(string.Format("{0}/default/Fieldtypes/{1}", "Forms/Themes", fieldTypeViewName));
        }

        public string GetReadOnlyFieldView(FormViewModel form, FieldViewModel field) => this.GetFieldView(form, field).Replace(".cshtml", ".ReadOnly.cshtml");

        private string GetThemedView(string view, string? theme) => this.GetThemedView(view, Guid.Empty, theme);

        private string GetThemedView(string view, Guid formId, string? theme)
        {
            if (formId != Guid.Empty)
            {
                string path = string.Format("{0}/{1}/{2}.cshtml", "Forms", formId.ToString().ToLowerInvariant(), view);
                if (this._partialViewFileSystem.FileExists(path))
                    return this._partialViewFileSystem.GetUrl(path);
            }
            string verifiedPath;
            return !string.IsNullOrEmpty(theme) && this.TryGetPathToView(theme, view, string.Empty, out verifiedPath) || this.TryGetPathToView(this.GetDefaultTheme(), string.Empty, view, out verifiedPath) ? verifiedPath : string.Format("{0}/{1}/{2}.cshtml", "/Views/Partials/Forms/Themes", "default", view);
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
            string defaultTheme = this._config.DefaultTheme;
            if (string.IsNullOrWhiteSpace(defaultTheme))
                defaultTheme = "default";
            return defaultTheme;
        }
    }
}
