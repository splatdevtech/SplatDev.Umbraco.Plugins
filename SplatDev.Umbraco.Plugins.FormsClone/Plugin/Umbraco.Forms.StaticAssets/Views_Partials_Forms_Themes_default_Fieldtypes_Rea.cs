
// Type: AspNetCoreGeneratedDocument.Views_Partials_Forms_Themes_default_Fieldtypes_ReadOnly
// Assembly: Umbraco.Forms.StaticAssets, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DDF8130F-541E-4CFE-9C57-74C2C790186A

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;

using System.Runtime.CompilerServices;

using Umbraco.Forms.Web.Models;


#nullable enable
namespace AspNetCoreGeneratedDocument
{
    [RazorSourceChecksum("Sha256", "56b9ce09e306bb67e811da2b5296fe8b3437dac617dc956ece80499e028eb390", "/Views/Partials/Forms/Themes/default/Fieldtypes/ReadOnly.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/ReadOnly.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_ReadOnly :
      RazorPage<FieldViewModel>
    {
#nullable disable
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_Fieldtypes_ReadOnly fieldtypesReadOnly = this;
            fieldtypesReadOnly.WriteLiteral("\n");
            fieldtypesReadOnly.Write(string.Join(", ", fieldtypesReadOnly.Model.Values.Select(x => GetDisplayValue(x))));
            fieldtypesReadOnly.WriteLiteral("\n\n");

            await Task.CompletedTask;
        }

        private
#nullable enable
    string GetDisplayValue(object value)
        {
            string stringValue = value.ToString();
            if (string.IsNullOrWhiteSpace(stringValue))
                return string.Empty;
            if (!this.Model.PreValues.Any<PrevalueViewModel>())
                return stringValue;
            PrevalueViewModel prevalueViewModel = this.Model.PreValues.FirstOrDefault<PrevalueViewModel>(x => x.Value == stringValue);
            return string.IsNullOrWhiteSpace(prevalueViewModel?.Caption) ? stringValue : prevalueViewModel.Caption;
        }

        [RazorInject]
        public IModelExpressionProvider ModelExpressionProvider { get; private set; }

        [RazorInject]
        public IUrlHelper Url { get; private set; }

        [RazorInject]
        public IViewComponentHelper Component { get; private set; }

        [RazorInject]
        public IJsonHelper Json { get; private set; }

        [RazorInject]
        public IHtmlHelper<FieldViewModel> Html { get; private set; }
    }
}
