using FormBuilder.Core.Extensions;
using FormBuilder.Core.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;

using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Templates;

namespace FormBuilder.StaticAssets
{
    //[RazorSourceChecksum("Sha256", "f88e44e460c37400003b7a9bc448b3d2563a3baa8e703f47ea4db20dd0dcc1f2", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.RichText.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.RichText.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_RichText :
      RazorPage<FieldViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_RichText fieldTypeRichText = this;
            fieldTypeRichText.WriteLiteral("\n");
            if (!fieldTypeRichText.Model.AdditionalSettings.TryGetValue("Html", out string? text))
                return;
            fieldTypeRichText.WriteLiteral("    <div");
            fieldTypeRichText.BeginWriteAttribute("id", " id=\"", 229, "\"", 243, 1);
            fieldTypeRichText.WriteAttributeValue("", 234, fieldTypeRichText.Model.Id, 234, 9, false);
            fieldTypeRichText.EndWriteAttribute();
            fieldTypeRichText.WriteLiteral(" data-umb=\"");
            fieldTypeRichText.Write(fieldTypeRichText.Model.Id);
            fieldTypeRichText.WriteLiteral("\"");
            fieldTypeRichText.BeginWriteAttribute("class", " class=\"", 265, "\"", 317, 1);
            fieldTypeRichText.WriteAttributeValue("", 273, fieldTypeRichText.Html?.GetFormFieldClass(fieldTypeRichText.Model.FieldTypeName), 273, 44, false);
            fieldTypeRichText.EndWriteAttribute();
            fieldTypeRichText.WriteLiteral(">\n        ");
            fieldTypeRichText.Write(fieldTypeRichText.Html?.Raw(fieldTypeRichText.HtmlLocalLinkParser?.EnsureInternalLinks(text)));
            fieldTypeRichText.WriteLiteral("\n    </div>\n");
            await Task.CompletedTask;
        }

        [RazorInject]
        public HtmlLocalLinkParser? HtmlLocalLinkParser
        { get; private set; }

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