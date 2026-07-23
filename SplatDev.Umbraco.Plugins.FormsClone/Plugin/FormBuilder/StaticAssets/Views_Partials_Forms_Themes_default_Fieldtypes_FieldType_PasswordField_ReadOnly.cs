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
    //[RazorSourceChecksum("Sha256", "799587cf5ea6638f5cc5d062e38aeecf01caca0d3a8c1affe7cee833aab9cd70", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.PasswordField.ReadOnly.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.PasswordField.ReadOnly.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_PasswordField_ReadOnly :
      RazorPage<FieldViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_PasswordField_ReadOnly passwordFieldReadOnly1 = this;
            passwordFieldReadOnly1.WriteLiteral("\n");
            Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_PasswordField_ReadOnly passwordFieldReadOnly2 = passwordFieldReadOnly1;
            string? str1 = passwordFieldReadOnly1.Model?.ValueAsHtmlString.Value;
            string str2 = string.Concat(Enumerable.Repeat("*", str1 is not null ? str1.Length : 0));
            passwordFieldReadOnly2.Write(str2);
            passwordFieldReadOnly1.WriteLiteral("\n");
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