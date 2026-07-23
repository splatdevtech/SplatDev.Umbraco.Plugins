using FormBuilder.Core.Configuration;
using FormBuilder.Core.Extensions;
using FormBuilder.Core.Models;
using FormBuilder.Web.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.Extensions.Options;

using System.Runtime.CompilerServices;

namespace FormBuilder.StaticAssets
{
    //[RazorSourceChecksum("Sha256", "a150360f49932c8d7c1061e3fe25fa1bcf02802df9867bfdc03185e4750ea2b5", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.Text.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.Text.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_Text :
      RazorPage<FieldViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_Text fieldtypesFieldTypeText = this;
            fieldtypesFieldTypeText.WriteLiteral("\n");
            fieldtypesFieldTypeText.WriteLiteral("\n");
            fieldtypesFieldTypeText.WriteLiteral("\n");
            IDictionary<string, string> additionalSettings = fieldtypesFieldTypeText.Model.AdditionalSettings;
            int num = !additionalSettings.TryGetValue("Caption", out string? value) ? 0 : !string.IsNullOrEmpty(value) ? 1 : 0;
            bool flag = additionalSettings.ContainsKey("BodyText") && !string.IsNullOrEmpty(additionalSettings["BodyText"]);
            string? settingValue = fieldtypesFieldTypeText.Model?.GetSettingValue("CaptionTag", "h2");
            fieldtypesFieldTypeText.WriteLiteral("\n<div");
            fieldtypesFieldTypeText.BeginWriteAttribute("id", " id=\"", 549, "\"", 563, 1);
            fieldtypesFieldTypeText.WriteAttributeValue("", 554, fieldtypesFieldTypeText.Model?.Id, 554, 9, false);
            fieldtypesFieldTypeText.EndWriteAttribute();
            fieldtypesFieldTypeText.WriteLiteral(" data-umb=\"");
            fieldtypesFieldTypeText.Write(fieldtypesFieldTypeText.Model?.Id);
            fieldtypesFieldTypeText.WriteLiteral("\"");
            fieldtypesFieldTypeText.BeginWriteAttribute("class", " class=\"", 585, "\"", 637, 1);
            fieldtypesFieldTypeText.WriteAttributeValue("", 593, fieldtypesFieldTypeText.Html?.GetFormFieldClass(fieldtypesFieldTypeText.Model?.FieldTypeName), 593, 44, false);
            fieldtypesFieldTypeText.EndWriteAttribute();
            fieldtypesFieldTypeText.WriteLiteral(">\n");
            if (num != 0)
            {
                fieldtypesFieldTypeText.Write(fieldtypesFieldTypeText.Html?.Raw("<" + settingValue + ">"));
                fieldtypesFieldTypeText.Write(additionalSettings["Caption"]);
                fieldtypesFieldTypeText.Write(fieldtypesFieldTypeText.Html?.Raw("</" + settingValue + ">"));
            }
            if (flag)
            {
                if (fieldtypesFieldTypeText.Configuration?.Value.AllowUnsafeHtmlRendering ?? false)
                {
                    fieldtypesFieldTypeText.WriteLiteral("            <p>");
                    fieldtypesFieldTypeText.Write(fieldtypesFieldTypeText.Html?.Raw(additionalSettings["BodyText"].Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "<br />")));
                    fieldtypesFieldTypeText.WriteLiteral("</p>\n");
                }
                else
                {
                    fieldtypesFieldTypeText.WriteLiteral("            <p>");
                    fieldtypesFieldTypeText.Write(additionalSettings["BodyText"]);
                    fieldtypesFieldTypeText.WriteLiteral("</p>\n");
                }
            }
            fieldtypesFieldTypeText.WriteLiteral("</div>\n");
            await Task.CompletedTask;
        }

        [RazorInject] public IOptionsSnapshot<TitleAndDescriptionSettings>? Configuration { get; private set; }

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