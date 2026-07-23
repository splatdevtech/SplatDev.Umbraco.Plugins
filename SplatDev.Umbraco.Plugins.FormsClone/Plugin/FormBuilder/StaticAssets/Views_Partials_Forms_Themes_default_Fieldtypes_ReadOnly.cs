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
    //[RazorSourceChecksum("Sha256", "56b9ce09e306bb67e811da2b5296fe8b3437dac617dc956ece80499e028eb390", "/Views/Partials/Forms/Themes/default/Fieldtypes/ReadOnly.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/ReadOnly.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_ReadOnly :
      RazorPage<FieldViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_Fieldtypes_ReadOnly fieldtypesReadOnly = this;
            fieldtypesReadOnly.WriteLiteral("\n");
            fieldtypesReadOnly.Write(string.Join(", ", fieldtypesReadOnly.Model.Values.Select(x => GetDisplayValue(x))));
            fieldtypesReadOnly.WriteLiteral("\n\n");

            await Task.CompletedTask;
        }

        private string GetDisplayValue(object value)
        {
            string? stringValue = value.ToString();
            if (string.IsNullOrWhiteSpace(stringValue))
                return string.Empty;
            if (!Model.PreValues.Any())
                return stringValue;
            PrevalueViewModel? prevalueViewModel = Model.PreValues.FirstOrDefault(x => x.Value == stringValue);
            return string.IsNullOrWhiteSpace(prevalueViewModel?.Caption) ? stringValue : prevalueViewModel.Caption;
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