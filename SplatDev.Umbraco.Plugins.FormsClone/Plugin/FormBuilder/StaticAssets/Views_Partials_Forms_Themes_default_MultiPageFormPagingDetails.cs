using FormBuilder.Core.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;

using System.Runtime.CompilerServices;
using System.Text;

namespace FormBuilder.StaticAssets
{
    [RazorSourceChecksum("Sha256", "bab021a0fbd5888ad65e63ff4d93a6fd41bcfe00a356f82dea826e1df652484a", "/Views/Partials/Forms/Themes/default/MultiPageFormPagingDetails.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/MultiPageFormPagingDetails.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_MultiPageFormPagingDetails :
      RazorPage<FormViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_MultiPageFormPagingDetails formPagingDetails = this;
            formPagingDetails.WriteLiteral("\n");
            StringBuilder stringBuilder = new();
            string format = formPagingDetails.Model.PagingDetailsFormat.Replace("{0}", "<span class=\"umbraco-forms-paging-count-number\">{0}</span>").Replace("{1}", "<span class=\"umbraco-forms-paging-count-number\">{1}</span>");
            stringBuilder.Append("<div class=\"umbraco-forms-paging-count\">");
            stringBuilder.AppendFormat(format, formPagingDetails.Model.PageNumber, formPagingDetails.Model.PageCount);
            stringBuilder.Append("</div>");
            stringBuilder.Append("<ol class=\"umbraco-forms-paging-captions\">");
            for (int index = 0; index < formPagingDetails.Model.Pages.Count; ++index)
                stringBuilder.AppendFormat("<li class=\"umbraco-forms-paging-captions-caption" + (formPagingDetails.Model.FormStep == index ? " umbraco-forms-paging-captions-caption-current" : string.Empty) + "\">{0}</li>", formPagingDetails.Model.GetPageCaption(index));
            if (formPagingDetails.Model.HasSummaryPage)
                stringBuilder.AppendFormat("<li class=\"umbraco-forms-paging-captions-caption\">{0}</li>", formPagingDetails.Model.SummaryCaption);
            stringBuilder.Append("</ol>");
            formPagingDetails.WriteLiteral("    <div class=\"umbraco-forms-paging\">\n        ");
            formPagingDetails.Write(formPagingDetails.Html?.Raw(stringBuilder.ToString()));
            formPagingDetails.WriteLiteral("\n    </div>\n");
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
        public IHtmlHelper<FormViewModel>? Html { get; private set; }
    }
}