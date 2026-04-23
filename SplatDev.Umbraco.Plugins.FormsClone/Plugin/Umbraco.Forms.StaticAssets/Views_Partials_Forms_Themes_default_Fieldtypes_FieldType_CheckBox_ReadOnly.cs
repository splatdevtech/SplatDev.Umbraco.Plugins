using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;

using System.Runtime.CompilerServices;

using Umbraco.Forms.Web.Models;

namespace UmbracoForms.StaticAssets
{
    [RazorSourceChecksum("Sha256", "7d411dcd01f4bf0299c7908cd464f05a7bd5d12369eb8a369f7b684966c7e6fa", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.CheckBox.ReadOnly.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.CheckBox.ReadOnly.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_CheckBox_ReadOnly :
      RazorPage<FieldViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_CheckBox_ReadOnly checkBoxReadOnly = this;
            checkBoxReadOnly.Write(checkBoxReadOnly.Model.ContainsValue(true) || checkBoxReadOnly.Model.ContainsValue("true") || checkBoxReadOnly.Model.ContainsValue("on") ? "Checked" : "Unchecked");
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