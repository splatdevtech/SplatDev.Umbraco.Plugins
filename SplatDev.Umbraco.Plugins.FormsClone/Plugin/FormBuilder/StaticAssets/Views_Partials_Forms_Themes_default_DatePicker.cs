using FormBuilder.Core.Configuration;
using FormBuilder.Core.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.Extensions.Options;

using System.Globalization;
using System.Runtime.CompilerServices;

namespace FormBuilder.StaticAssets
{
    //[RazorSourceChecksum("Sha256", "d59df456ce3d0dc1033175a147923671f2de27109255b0bba5fb158a3cff7f05", "/Views/Partials/Forms/Themes/default/DatePicker.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/DatePicker.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_DatePicker : RazorPage<object>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_DatePicker defaultDatePicker = this;
            defaultDatePicker.WriteLiteral("\n");
            defaultDatePicker.WriteLiteral("\n");
            int? datePickerYearRange = defaultDatePicker.Configuration?.Value.DatePickerYearRange;
            string? str = defaultDatePicker.Configuration?.Value.DatePickerFormat;
            if (string.IsNullOrWhiteSpace(str))
                str = "LL";
            defaultDatePicker.Html?.AddFormThemeCssFile("~/App_Plugins/FormBuilder/assets/pikaday/pikaday.min.css");
            defaultDatePicker.Html?.AddFormThemeScriptFile("~/App_Plugins/FormBuilder/assets/moment/min/moment-with-locales.min.js");
            defaultDatePicker.Html?.AddFormThemeScriptFile("~/App_Plugins/FormBuilder/assets/pikaday/pikaday.min.js");
            defaultDatePicker.Html?.AddFormThemeScriptFile("~/App_Plugins/FormBuilder/assets/datepicker.init.min.js");
            bool flag = defaultDatePicker.Context.Items.ContainsKey("__formDatePickerRendered");
            defaultDatePicker.Context.Items["__formDatePickerRendered"] = true;
            defaultDatePicker.WriteLiteral("\n");
            if (flag)
                return;

            defaultDatePicker.WriteLiteral("    <div id=\"umbraco-forms-date-picker-config\"\n         class=\"umbraco-forms-hidden\"\n         data-name=\"");
            defaultDatePicker.Write(CultureInfo.CurrentUICulture.Name);
            defaultDatePicker.WriteLiteral("\"\n         data-year-range=\"");
            defaultDatePicker.Write(datePickerYearRange);
            defaultDatePicker.WriteLiteral("\"\n         data-format=\"");
            defaultDatePicker.Write(str);
            defaultDatePicker.WriteLiteral("\"\n         data-previous-month=\"<<\"\n         data-next-month=\">>\"\n         data-months=\"");
            defaultDatePicker.Write(string.Join(",", CultureInfo.CurrentCulture.DateTimeFormat.MonthNames));
            defaultDatePicker.WriteLiteral("\"\n         data-weekdays=\"");
            defaultDatePicker.Write(string.Join(",", CultureInfo.CurrentCulture.DateTimeFormat.DayNames));
            defaultDatePicker.WriteLiteral("\"\n         data-weekdays-short=\"");
            defaultDatePicker.Write(string.Join(",", CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames));
            defaultDatePicker.WriteLiteral("\"></div>\n");
            await Task.CompletedTask;
        }

        [RazorInject]
        public IOptionsSnapshot<DatePickerSettings>? Configuration { get; private set; }

        [RazorInject]
        public IModelExpressionProvider? ModelExpressionProvider { get; private set; }

        [RazorInject]
        public IUrlHelper? Url { get; private set; }

        [RazorInject]
        public IViewComponentHelper? Component { get; private set; }

        [RazorInject]
        public IJsonHelper? Json { get; private set; }

        [RazorInject]
        public IHtmlHelper<object>? Html { get; private set; }
    }
}