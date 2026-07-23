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
    //[RazorSourceChecksum("Sha256", "8d1115e65426a83c2dc4320810ef39bca3bc85d6379995dec873e5ee308c983f", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.CheckBoxList.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.CheckBoxList.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_CheckBoxList :
      RazorPage<FieldViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_CheckBoxList typeCheckBoxList = this;
            typeCheckBoxList.WriteLiteral("\n");
            int num = 0;
            string lower = (typeCheckBoxList.Model.GetSettingValue("DisplayLayout", "Vertical") ?? string.Empty).ToLower();
            typeCheckBoxList.WriteLiteral("          \n<div");
            typeCheckBoxList.BeginWriteAttribute("class", " class=\"", 220, "\"", 268, 3);
            typeCheckBoxList.WriteAttributeValue("", 228, "checkboxlist", 228, 12, true);
            typeCheckBoxList.WriteAttributeValue(" ", 240, "checkboxlist-", 241, 14, true);
            typeCheckBoxList.WriteAttributeValue("", 254, lower, 254, 14, false);
            typeCheckBoxList.EndWriteAttribute();
            typeCheckBoxList.BeginWriteAttribute("id", " id=\"", 269, "\"", 283, 1);
            typeCheckBoxList.WriteAttributeValue("", 274, typeCheckBoxList.Model.Id, 274, 9, false);
            typeCheckBoxList.EndWriteAttribute();
            typeCheckBoxList.WriteLiteral(" data-umb=\"");
            typeCheckBoxList.Write(typeCheckBoxList.Model.Id);
            typeCheckBoxList.WriteLiteral("\">\n");
            foreach (PrevalueViewModel preValue in typeCheckBoxList.Model.PreValues)
            {
                typeCheckBoxList.WriteLiteral("        <div>\n            <input type=\"checkbox\"");
                typeCheckBoxList.BeginWriteAttribute("class", "\n                   class=\"", 441, "\"", 512, 1);
                typeCheckBoxList.WriteAttributeValue("", 468, typeCheckBoxList.Html?.GetFormFieldClass(typeCheckBoxList.Model.FieldTypeName), 468, 44, false);
                typeCheckBoxList.EndWriteAttribute();
                typeCheckBoxList.BeginWriteAttribute("name", "\n                   name=\"", 513, "\"", 550, 1);
                typeCheckBoxList.WriteAttributeValue("", 539, typeCheckBoxList.Model.Name, 539, 11, false);
                typeCheckBoxList.EndWriteAttribute();
                typeCheckBoxList.BeginWriteAttribute("id", " id=\"", 551, "\"", 586, 1);
                typeCheckBoxList.WriteAttributeValue("", 556, typeCheckBoxList.Model.Id + "_" + num, 556, 30, false);
                typeCheckBoxList.EndWriteAttribute();
                typeCheckBoxList.WriteLiteral(" data-umb=\"");
                typeCheckBoxList.Write(typeCheckBoxList.Model.Id + "_" + num);
                typeCheckBoxList.WriteLiteral("\"");
                typeCheckBoxList.BeginWriteAttribute("value", " value=\"", 631, "\"", 648, 1);
                typeCheckBoxList.WriteAttributeValue("", 639, preValue.Value, 639, 9, false);
                typeCheckBoxList.EndWriteAttribute();
                typeCheckBoxList.WriteLiteral("\n            ");
                if (typeCheckBoxList.Model.Mandatory)
                {
                    typeCheckBoxList.WriteLiteral("data-val=\"true\" data-val-required=\"");
                    typeCheckBoxList.Write(typeCheckBoxList.Model.RequiredErrorMessage);
                    typeCheckBoxList.WriteLiteral("\" data-rule-required=\"true\" data-msg-required=\"");
                    typeCheckBoxList.Write(typeCheckBoxList.Model.RequiredErrorMessage);
                    typeCheckBoxList.WriteLiteral("\"");
                }
                typeCheckBoxList.WriteLiteral("            ");
                if (typeCheckBoxList.Model.ContainsValue(preValue.Value))
                    typeCheckBoxList.WriteLiteral("checked=\"checked\"");
                typeCheckBoxList.WriteLiteral(" />\n\n            <label");
                typeCheckBoxList.BeginWriteAttribute("for", " for=\"", 1024, "\"", 1060, 1);
                typeCheckBoxList.WriteAttributeValue("", 1030, typeCheckBoxList.Model.Id + "_" + num, 1030, 30, false);
                typeCheckBoxList.EndWriteAttribute();
                typeCheckBoxList.WriteLiteral(">");
                typeCheckBoxList.Write(preValue.Caption);
                typeCheckBoxList.WriteLiteral("</label>\n        </div>\n");
                ++num;
            }
            typeCheckBoxList.WriteLiteral("</div>\n");
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