using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;

using System.Runtime.CompilerServices;

using Umbraco.Forms.Web;
using Umbraco.Forms.Web.Models;

namespace UmbracoForms.StaticAssets
{
    //[RazorSourceChecksum("Sha256", "b882bf388dea13d486a96d3d29fa91279cd7e675eba6bc4d37ca08712c514bf4", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.DropDownList.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.DropDownList.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_DropDownList :
      RazorPage<FieldViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_DropDownList typeDropDownList = this;
            IDictionary<string, string> additionalSettings = typeDropDownList.Model.AdditionalSettings;
            string? settingValue1 = typeDropDownList?.Model?.GetSettingValue("AutocompleteAttribute", string.Empty);
            int num = !additionalSettings.TryGetValue("AllowMultipleSelections", out string? value) ? 0 : value.Equals("true", StringComparison.CurrentCultureIgnoreCase) ? 1 : 0;
            string? settingValue2 = typeDropDownList?.Model?.GetSettingValue("SelectPrompt", string.Empty);
            typeDropDownList?.WriteLiteral("\n<select");
            typeDropDownList?.BeginWriteAttribute("class", " class=\"", 448, "\"", 500, 1);
            typeDropDownList?.WriteAttributeValue("", 456, typeDropDownList?.Html?.GetFormFieldClass(typeDropDownList?.Model?.FieldTypeName), 456, 44, false);
            typeDropDownList?.EndWriteAttribute();
            typeDropDownList?.BeginWriteAttribute("name", "\n        name=\"", 501, "\"", 527, 1);
            typeDropDownList?.WriteAttributeValue("", 516, typeDropDownList?.Model?.Name, 516, 11, false);
            typeDropDownList?.EndWriteAttribute();
            typeDropDownList?.BeginWriteAttribute("id", "\n        id=\"", 528, "\"", 550, 1);
            typeDropDownList?.WriteAttributeValue("", 541, typeDropDownList?.Model?.Id, 541, 9, false);
            typeDropDownList?.EndWriteAttribute();
            typeDropDownList?.WriteLiteral("\n        data-umb=\"");
            typeDropDownList?.Write(typeDropDownList?.Model.Id);
            typeDropDownList?.WriteLiteral("\"\n        ");
            if (!string.IsNullOrEmpty(settingValue1))
            {
                typeDropDownList?.WriteLiteral("autocomplete=\"");
                typeDropDownList?.Write(settingValue1);
                typeDropDownList?.WriteLiteral("\"");
            }
            typeDropDownList?.WriteLiteral("        ");
            if (num != 0)
                typeDropDownList?.WriteLiteral(" multiple ");
            typeDropDownList?.WriteLiteral("        ");
            if (typeDropDownList is not null && typeDropDownList.Model.Mandatory)
            {
                typeDropDownList?.WriteLiteral(" data-val=\"true\" data-val-required=\"");
                typeDropDownList?.Write(typeDropDownList?.Model.RequiredErrorMessage);
                typeDropDownList?.WriteLiteral("\" aria-required=\"true\" ");
            }
            typeDropDownList?.WriteLiteral("        ");
            if (!string.IsNullOrEmpty(typeDropDownList?.Model.ToolTip))
            {
                typeDropDownList?.WriteLiteral(" aria-describedby=\"");
                typeDropDownList?.Write(typeDropDownList?.Model.Id);
                typeDropDownList?.WriteLiteral("_description\" ");
            }
            typeDropDownList?.WriteLiteral(">\n");
            if (num == 0)
            {
                typeDropDownList?.WriteLiteral("        <option");
                typeDropDownList?.BeginWriteAttribute("value", " value=\"", 1053, "\"", 1061, 0);
                typeDropDownList?.EndWriteAttribute();
                typeDropDownList?.WriteLiteral(">");
                typeDropDownList?.Write(settingValue2);
                typeDropDownList?.WriteLiteral("</option>\n");
            }
            foreach (PrevalueViewModel? preValue in typeDropDownList!.Model!.PreValues)
            {
                typeDropDownList?.WriteLiteral("        ");
                typeDropDownList?.WriteLiteral(" <option value=\"");
                typeDropDownList?.Write(preValue.Value);
                typeDropDownList?.WriteLiteral("\" ");
                typeDropDownList?.Write(typeDropDownList.Model.ContainsValue(preValue.Value) ? "selected" : string.Empty);
                typeDropDownList?.WriteLiteral(">");
                typeDropDownList?.Write(preValue.Caption);
                typeDropDownList?.WriteLiteral("</option>\n");
            }
            typeDropDownList?.WriteLiteral("</select>\n");
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