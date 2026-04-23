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
    //[RazorSourceChecksum("Sha256", "7250d1669a7a14ec5e83c5d7a38c4f8f7c6f5bb5f41fc873e7c424c892aed4e2", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.DataConsent.ReadOnly.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.DataConsent.ReadOnly.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_DataConsent_ReadOnly :
      RazorPage<FieldViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_DataConsent_ReadOnly dataConsentReadOnly = this;
            dataConsentReadOnly.Write(dataConsentReadOnly.Model.ContainsValue(true) || dataConsentReadOnly.Model.ContainsValue("true") || dataConsentReadOnly.Model.ContainsValue("on") ? "Agreed" : "Not agreed");
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