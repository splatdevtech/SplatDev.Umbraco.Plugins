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
    //[RazorSourceChecksum("Sha256", "87ffb6bbf8bbcc1e0239d8da870ac48a9f154c03ad1adf1133ee5006dbc25324", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.RadioButtonList.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.RadioButtonList.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_RadioButtonList :
      RazorPage<FieldViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_RadioButtonList typeRadioButtonList = this;
            typeRadioButtonList.WriteLiteral("  \n");
            int num = 0;
            string lower = (typeRadioButtonList.Model.GetSettingValue("DisplayLayout", "Vertical") ?? string.Empty).ToLower();
            typeRadioButtonList.WriteLiteral("   \n<div");
            typeRadioButtonList.BeginWriteAttribute("class", " class=\"", 217, "\"", 316, 4);
            typeRadioButtonList.WriteAttributeValue("", 225, typeRadioButtonList.Html?.GetFormFieldClass(typeRadioButtonList.Model.FieldTypeName), 225, 44, false);
            typeRadioButtonList.WriteAttributeValue(" ", 269, "radiobuttonlist", 270, 16, true);
            typeRadioButtonList.WriteAttributeValue(" ", 285, "radiobuttonlist-", 286, 17, true);
            typeRadioButtonList.WriteAttributeValue("", 302, lower, 302, 14, false);
            typeRadioButtonList.EndWriteAttribute();
            typeRadioButtonList.BeginWriteAttribute("id", " id=\"", 317, "\"", 331, 1);
            typeRadioButtonList.WriteAttributeValue("", 322, typeRadioButtonList.Model.Id, 322, 9, false);
            typeRadioButtonList.EndWriteAttribute();
            typeRadioButtonList.WriteLiteral(" data-umb=\"");
            typeRadioButtonList.Write(typeRadioButtonList.Model.Id);
            typeRadioButtonList.WriteLiteral("\">\n");
            foreach (PrevalueViewModel preValue in typeRadioButtonList.Model.PreValues)
            {
                typeRadioButtonList.WriteLiteral("        <div>\n            <input type=\"radio\"");
                typeRadioButtonList.BeginWriteAttribute("name", " name=\"", 486, "\"", 504, 1);
                typeRadioButtonList.WriteAttributeValue("", 493, typeRadioButtonList.Model.Name, 493, 11, false);
                typeRadioButtonList.EndWriteAttribute();
                typeRadioButtonList.BeginWriteAttribute("id", " id=\"", 505, "\"", 540, 1);
                typeRadioButtonList.WriteAttributeValue("", 510, typeRadioButtonList.Model.Id + "_" + num, 510, 30, false);
                typeRadioButtonList.EndWriteAttribute();
                typeRadioButtonList.WriteLiteral(" data-umb=\"");
                typeRadioButtonList.Write(typeRadioButtonList.Model.Id + "_" + num);
                typeRadioButtonList.WriteLiteral("\"");
                typeRadioButtonList.BeginWriteAttribute("value", " value=\"", 585, "\"", 602, 1);
                typeRadioButtonList.WriteAttributeValue("", 593, preValue.Value, 593, 9, false);
                typeRadioButtonList.EndWriteAttribute();
                typeRadioButtonList.WriteLiteral("\n            ");
                if (typeRadioButtonList.Model.Mandatory)
                {
                    typeRadioButtonList.WriteLiteral("data-val=\"true\" data-val-required=\"");
                    typeRadioButtonList.Write(typeRadioButtonList.Model.RequiredErrorMessage);
                    typeRadioButtonList.WriteLiteral("\" data-rule-required=\"true\" data-msg-required=\"");
                    typeRadioButtonList.Write(typeRadioButtonList.Model.RequiredErrorMessage);
                    typeRadioButtonList.WriteLiteral("\"");
                }
                typeRadioButtonList.WriteLiteral("            ");
                if (typeRadioButtonList.Model.ContainsValue(preValue.Value))
                    typeRadioButtonList.WriteLiteral("checked=\"checked\"");
                typeRadioButtonList.WriteLiteral(" />\n            <label");
                typeRadioButtonList.BeginWriteAttribute("for", " for=\"", 977, "\"", 1013, 1);
                typeRadioButtonList.WriteAttributeValue("", 983, typeRadioButtonList.Model.Id + "_" + num, 983, 30, false);
                typeRadioButtonList.EndWriteAttribute();
                typeRadioButtonList.WriteLiteral(">");
                typeRadioButtonList.Write(preValue.Caption);
                typeRadioButtonList.WriteLiteral("</label>\n        </div>\n");
                ++num;
            }
            typeRadioButtonList.WriteLiteral("</div>\n");
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