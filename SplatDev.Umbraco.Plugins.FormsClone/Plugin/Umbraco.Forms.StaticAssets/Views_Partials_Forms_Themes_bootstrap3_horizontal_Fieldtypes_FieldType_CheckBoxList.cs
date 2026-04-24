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
    //[RazorSourceChecksum("Sha256", "6d3a0e44709227882a0a61116b00ef3311922ab7342104e813c34edd57908e88", "/Views/Partials/Forms/Themes/bootstrap3-horizontal/Fieldtypes/FieldType.CheckBoxList.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/bootstrap3-horizontal/Fieldtypes/FieldType.CheckBoxList.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_bootstrap3_horizontal_Fieldtypes_FieldType_CheckBoxList :
      RazorPage<FieldViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_bootstrap3_horizontal_Fieldtypes_FieldType_CheckBoxList typeCheckBoxList = this;
            typeCheckBoxList.WriteLiteral("\n");
            int num = 0;
            string lower = (typeCheckBoxList.Model.GetSettingValue("DisplayLayout", "Vertical") ?? string.Empty).ToLower();
            typeCheckBoxList.WriteLiteral("\n<div class=\"checkboxlist\"");
            typeCheckBoxList.BeginWriteAttribute("id", " id=\"", 231, "\"", 245, 1);
            typeCheckBoxList.WriteAttributeValue("", 236, typeCheckBoxList.Model.Id, 236, 9, false);
            typeCheckBoxList.EndWriteAttribute();
            typeCheckBoxList.WriteLiteral(" data-umb=\"");
            typeCheckBoxList.Write(typeCheckBoxList.Model.Id);
            typeCheckBoxList.WriteLiteral("\">\n");
            foreach (PrevalueViewModel preValue in typeCheckBoxList.Model.PreValues)
            {
                typeCheckBoxList.WriteLiteral("        <div");
                typeCheckBoxList.BeginWriteAttribute("class", " class=\"", 367, "\"", 456, 2);
                typeCheckBoxList.WriteAttributeValue("", 375, "form-check", 375, 10, true);
                typeCheckBoxList.WriteAttributeValue("", 385, lower == "horizontal" ? " form-check-inline" : (object)string.Empty, 385, 71, false);
                typeCheckBoxList.EndWriteAttribute();
                typeCheckBoxList.WriteLiteral(">\n            <input type=\"checkbox\"\n                   class=\"form-check-input\"");
                typeCheckBoxList.BeginWriteAttribute("name", "\n                   name=\"", 537, "\"", 574, 1);
                typeCheckBoxList.WriteAttributeValue("", 563, typeCheckBoxList.Model.Name, 563, 11, false);
                typeCheckBoxList.EndWriteAttribute();
                typeCheckBoxList.BeginWriteAttribute("id", " id=\"", 575, "\"", 610, 1);
                typeCheckBoxList.WriteAttributeValue("", 580, typeCheckBoxList.Model.Id + "_" + num, 580, 30, false);
                typeCheckBoxList.EndWriteAttribute();
                typeCheckBoxList.WriteLiteral(" data-umb=\"");
                typeCheckBoxList.Write(typeCheckBoxList.Model.Id + "_" + num);
                typeCheckBoxList.WriteLiteral("\"");
                typeCheckBoxList.BeginWriteAttribute("value", " value=\"", 655, "\"", 672, 1);
                typeCheckBoxList.WriteAttributeValue("", 663, preValue.Value, 663, 9, false);
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
                typeCheckBoxList.WriteLiteral(" />\n\n            <label class=\"form-check-label\"");
                typeCheckBoxList.BeginWriteAttribute("for", " for=\"", 1073, "\"", 1109, 1);
                typeCheckBoxList.WriteAttributeValue("", 1079, typeCheckBoxList.Model.Id + "_" + num, 1079, 30, false);
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