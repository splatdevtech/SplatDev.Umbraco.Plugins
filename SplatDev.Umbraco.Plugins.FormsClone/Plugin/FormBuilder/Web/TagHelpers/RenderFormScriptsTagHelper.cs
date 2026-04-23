using FormBuilder.Core.Configuration;
using FormBuilder.Web.Extensions;
using FormBuilder.Web.ViewComponents;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace FormBuilder.Web.TagHelpers
{
    /// <summary>
    /// A tag helper to render the scripts for Umbraco Forms forms.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    /// <param name="viewComponentHelper">The view component helper.</param>
    /// <param name="packageOptionSettings">The configured package options settings.</param>
    [HtmlTargetElement("umb-forms-render-scripts")]
    public class RenderFormScriptsTagHelper(
      IViewComponentHelper viewComponentHelper,
      IOptions<PackageOptionSettings> packageOptionSettings) : TagHelper
    {
        private readonly IViewComponentHelper _viewComponentHelper = viewComponentHelper;
        private readonly PackageOptionSettings _packageOptionSettings = packageOptionSettings.Value;

        /// <summary>
        /// Gets or sets the form ID to render (gets all rendered forms from TempData if omitted).
        /// </summary>
        [HtmlAttributeName("form-id")]
        public Guid? FormId { get; set; }

        /// <summary>
        /// Gets or sets the name of the theme to use to render the form.
        /// </summary>
        [HtmlAttributeName("theme")]
        public string? Theme { get; set; }

        /// <summary>Gets or sets the view context.</summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; } = new ViewContext();

        /// <inheritdoc />
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            ((IViewContextAware)_viewComponentHelper).Contextualize(ViewContext);
            output.SuppressOutput();
            Guid? formId1 = FormId;
            if (formId1.HasValue)
            {
                Guid valueOrDefault = formId1.GetValueOrDefault();
                await RenderFormScriptsAsync(output, valueOrDefault);
            }
            else
            {
                if (_packageOptionSettings.TrackRenderedFormsStorageMethod == TrackRenderedFormsStorageMethodOption.TempData)
                {
                    IEnumerable<Guid>? guids = ViewContext.TempData.Get<List<Guid>>("FormBuilder");
                    if (guids is not null)
                    {
                        ViewContext.TempData.Remove("FormBuilder");
                        foreach (Guid formId2 in guids)
                            await RenderFormScriptsAsync(output, formId2);
                        return;
                    }
                }
                if (_packageOptionSettings.TrackRenderedFormsStorageMethod != TrackRenderedFormsStorageMethodOption.HttpContextItems || !ViewContext.HttpContext.Items.TryGetValue("FormBuilder", out object? obj) || obj is not IEnumerable<Guid> guids1)
                    return;
                foreach (Guid formId3 in guids1)
                    await RenderFormScriptsAsync(output, formId3);
            }
        }

        private async Task RenderFormScriptsAsync(TagHelperOutput output, Guid formId) => output.PostElement.AppendHtml(await _viewComponentHelper.InvokeAsync(typeof(RenderFormScriptsViewComponent), new
        {
            formId,
            theme = Theme
        }));
    }
}