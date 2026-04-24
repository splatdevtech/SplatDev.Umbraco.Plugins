using FormBuilder.Core.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;

using System.Runtime.CompilerServices;

namespace FormBuilder.StaticAssets
{
    [RazorSourceChecksum("Sha256", "44422e15c15caefe6e75dc8b75ebcc64b1e5ec1377a298e67697c7932a71904d", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.CheckBox.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.CheckBox.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_CheckBox :
      RazorPage<FieldViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_CheckBox fieldTypeCheckBox = this;
            fieldTypeCheckBox.Model.AdditionalSettings.TryGetValue("Caption", out var _);
            fieldTypeCheckBox.WriteLiteral("\n<input type=\"checkbox\"");
            fieldTypeCheckBox.BeginWriteAttribute("name", " name=\"", 173, "\"", 191, 1);
            fieldTypeCheckBox.WriteAttributeValue("", 180, fieldTypeCheckBox.Model.Name, 180, 11, false);
            fieldTypeCheckBox.EndWriteAttribute();
            fieldTypeCheckBox.BeginWriteAttribute("id", " id=\"", 192, "\"", 206, 1);
            fieldTypeCheckBox.WriteAttributeValue("", 197, fieldTypeCheckBox.Model.Id, 197, 9, false);
            fieldTypeCheckBox.EndWriteAttribute();
            fieldTypeCheckBox.WriteLiteral(" value=\"true\"  data-umb=\"");
            fieldTypeCheckBox.Write(fieldTypeCheckBox.Model.Id);
            fieldTypeCheckBox.WriteLiteral("\"\n       ");
            if (fieldTypeCheckBox.Model.Mandatory)
            {
                fieldTypeCheckBox.WriteLiteral("  data-val=\"true\" data-val-requiredcb=\"");
                fieldTypeCheckBox.Write(fieldTypeCheckBox.Model.RequiredErrorMessage);
                fieldTypeCheckBox.WriteLiteral("\" aria-required=\"true\"");
            }
            fieldTypeCheckBox.WriteLiteral("       ");
            if (fieldTypeCheckBox.Model.ContainsValue(true) || fieldTypeCheckBox.Model.ContainsValue("true") || fieldTypeCheckBox.Model.ContainsValue("on"))
                fieldTypeCheckBox.WriteLiteral("checked=\"checked\"");
            fieldTypeCheckBox.WriteLiteral("       ");
            if (!string.IsNullOrEmpty(fieldTypeCheckBox.Model.ToolTip))
            {
                fieldTypeCheckBox.WriteLiteral(" aria-describedby=\"");
                fieldTypeCheckBox.Write(fieldTypeCheckBox.Model.Id);
                fieldTypeCheckBox.WriteLiteral("_description\" ");
            }
            fieldTypeCheckBox.WriteLiteral("/>\n<input type=\"hidden\"");
            fieldTypeCheckBox.BeginWriteAttribute("name", " name=\"", 807, "\"", 825, 1);
            fieldTypeCheckBox.WriteAttributeValue("", 814, fieldTypeCheckBox.Model.Name, 814, 11, false);
            fieldTypeCheckBox.EndWriteAttribute();
            fieldTypeCheckBox.WriteLiteral(" value=\"false\" />\n");
            if (string.IsNullOrEmpty(string.Empty))
                return;
            fieldTypeCheckBox.WriteLiteral("    <label");
            fieldTypeCheckBox.BeginWriteAttribute("for", " for=\"", 893, "\"", 908, 1);
            fieldTypeCheckBox.WriteAttributeValue("", 899, fieldTypeCheckBox.Model.Id, 899, 9, false);
            fieldTypeCheckBox.EndWriteAttribute();
            fieldTypeCheckBox.WriteLiteral(">");
            fieldTypeCheckBox.Write(string.Empty);
            fieldTypeCheckBox.WriteLiteral("</label>\n");
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