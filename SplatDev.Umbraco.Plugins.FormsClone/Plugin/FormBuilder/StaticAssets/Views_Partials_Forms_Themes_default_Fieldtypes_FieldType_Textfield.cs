using FormBuilder.Core.Extensions;
using FormBuilder.Core.Models;
using FormBuilder.Web.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;

using System.Runtime.CompilerServices;

namespace FormBuilder.StaticAssets
{
    //[RazorSourceChecksum("Sha256", "88e915f430924741465eba89ef910c2f073111c68bbb22a3bbbdf661daa1dc0c", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.Textfield.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.Textfield.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_Textfield :
      RazorPage<FieldViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_Textfield fieldTypeTextfield = this;
            int settingValue1 = fieldTypeTextfield.Model.GetSettingValue<int>("MaximumLength", byte.MaxValue);
            string? settingValue2 = fieldTypeTextfield.Model.GetSettingValue("FieldType", "text");
            string? settingValue3 = fieldTypeTextfield.Model.GetSettingValue("AutocompleteAttribute", string.Empty);
            fieldTypeTextfield.WriteLiteral("<input");
            fieldTypeTextfield.BeginWriteAttribute("type", " type=\"", 327, "\"", 344, 1);
            fieldTypeTextfield.WriteAttributeValue("", 334, settingValue2, 334, 10, false);
            fieldTypeTextfield.EndWriteAttribute();
            fieldTypeTextfield.BeginWriteAttribute("name", " name=\"", 345, "\"", 363, 1);
            fieldTypeTextfield.WriteAttributeValue("", 352, fieldTypeTextfield.Model.Name, 352, 11, false);
            fieldTypeTextfield.EndWriteAttribute();
            fieldTypeTextfield.BeginWriteAttribute("id", " id=\"", 364, "\"", 378, 1);
            fieldTypeTextfield.WriteAttributeValue("", 369, fieldTypeTextfield.Model.Id, 369, 9, false);
            fieldTypeTextfield.EndWriteAttribute();
            fieldTypeTextfield.WriteLiteral(" data-umb=\"");
            fieldTypeTextfield.Write(fieldTypeTextfield.Model.Id);
            fieldTypeTextfield.WriteLiteral("\"");
            fieldTypeTextfield.BeginWriteAttribute("class", " class=\"", 400, "\"", 457, 2);
            fieldTypeTextfield.WriteAttributeValue("", 408, "text", 408, 4, true);
            fieldTypeTextfield.WriteAttributeValue(" ", 412, fieldTypeTextfield.Html?.GetFormFieldClass(fieldTypeTextfield.Model.FieldTypeName), 413, 44, false);
            fieldTypeTextfield.EndWriteAttribute();
            fieldTypeTextfield.BeginWriteAttribute("value", " value=\"", 458, "\"", 490, 1);
            fieldTypeTextfield.WriteAttributeValue("", 466, fieldTypeTextfield.Model.ValueAsHtmlString, 466, 24, false);
            fieldTypeTextfield.EndWriteAttribute();
            fieldTypeTextfield.BeginWriteAttribute("maxlength", " maxlength=\"", 491, "\"", 513, 1);
            fieldTypeTextfield.WriteAttributeValue("", 503, settingValue1, 503, 10, false);
            fieldTypeTextfield.EndWriteAttribute();
            fieldTypeTextfield.WriteLiteral("\n       ");
            if (!string.IsNullOrEmpty(fieldTypeTextfield.Model.PlaceholderText))
            {
                fieldTypeTextfield.WriteLiteral(" placeholder=\"");
                fieldTypeTextfield.Write(fieldTypeTextfield.Model.PlaceholderText);
                fieldTypeTextfield.WriteLiteral("\" ");
            }
            fieldTypeTextfield.WriteLiteral("\n       ");
            if (!string.IsNullOrEmpty(settingValue3))
            {
                fieldTypeTextfield.WriteLiteral("autocomplete=\"");
                fieldTypeTextfield.Write(settingValue3);
                fieldTypeTextfield.WriteLiteral("\" ");
            }
            fieldTypeTextfield.WriteLiteral("\n       ");
            if (fieldTypeTextfield.Model.Mandatory || fieldTypeTextfield.Model.Validate)
                fieldTypeTextfield.WriteLiteral("data-val=\"true\" ");
            fieldTypeTextfield.WriteLiteral("\n       ");
            if (fieldTypeTextfield.Model.Mandatory)
            {
                fieldTypeTextfield.WriteLiteral("data-val-required=\"");
                fieldTypeTextfield.Write(fieldTypeTextfield.Model.RequiredErrorMessage);
                fieldTypeTextfield.WriteLiteral("\" aria-required=\"true\" ");
            }
            fieldTypeTextfield.WriteLiteral("\n       ");
            if (fieldTypeTextfield.Model.Validate)
            {
                fieldTypeTextfield.WriteLiteral("data-val-regex=\"");
                fieldTypeTextfield.Write(fieldTypeTextfield.Model.InvalidErrorMessage);
                fieldTypeTextfield.WriteLiteral("\" data-val-regex-pattern=\"");
                fieldTypeTextfield.Write(fieldTypeTextfield.Html?.Raw(fieldTypeTextfield.Model.Regex));
                fieldTypeTextfield.WriteLiteral("\" ");
            }
            fieldTypeTextfield.WriteLiteral("\n       ");
            if (!string.IsNullOrEmpty(fieldTypeTextfield.Model.ToolTip))
            {
                fieldTypeTextfield.WriteLiteral("aria-describedby=\"");
                fieldTypeTextfield.Write(fieldTypeTextfield.Model.Id);
                fieldTypeTextfield.WriteLiteral("_description\" ");
            }
            fieldTypeTextfield.WriteLiteral("/>\n\n\n\n");

            await Task.CompletedTask;
        }

        [RazorInject]
        public IModelExpressionProvider? ModelExpressionProvider { get; private set; }

        [RazorInject]
        public IUrlHelper? Url { get; private set; }

        [RazorInject]
        public IViewComponentHelper? Component { get; private set; }

        [RazorInject]
        public IJsonHelper? Json { get; private set; }

        [RazorInject]
        public IHtmlHelper<FieldViewModel>? Html { get; private set; }
    }
}