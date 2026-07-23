using FormBuilder.Core.Searches;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;

using System.Runtime.CompilerServices;

namespace FormBuilder.StaticAssets
{
    //[RazorSourceChecksum("Sha256", "87d8e09be811576e158625f5d873f89c61ff43aa9f62107f98225dcc59fc7039", "/Views/Partials/Forms/Export/excel.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Export/excel.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Export_excel : RazorPage<EntrySearchResultCollection>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Export_excel formsExportExcel = this;
            foreach (EntrySearchResultSchema searchResultSchema in formsExportExcel.Model.Schema)
            {
                formsExportExcel.WriteLiteral("\"");
                formsExportExcel.Write(formsExportExcel.Html?.Raw(searchResultSchema.Name));
                formsExportExcel.WriteLiteral("\",");
            }
            formsExportExcel.WriteLiteral("\n");
            foreach (EntrySearchResult result in formsExportExcel.Model.Results)
            {
                foreach (EntrySearchResult.FieldData field in result.Fields)
                {
                    formsExportExcel.WriteLiteral("\"");
                    formsExportExcel.Write(formsExportExcel.Html?.Raw(field.Value).ToString()?.Replace(Environment.NewLine, " "));
                    formsExportExcel.WriteLiteral("\",");
                }
                formsExportExcel.Write(Environment.NewLine);
            }
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
        public IHtmlHelper<EntrySearchResultCollection>? Html { get; private set; }
    }
}