using FormBuilder.Core.Configuration;
using FormBuilder.Web.ViewComponents;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace FormBuilder.Web.TagHelpers
{
    /// <summary>A tag helper to render an Umbraco Forms form.</summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    /// <param name="viewComponentHelper">The view component helper.</param>
    /// <param name="packageOptionSettings">The package option settings.</param>
    [HtmlTargetElement("form-builder-render")]
    public class RenderFormTagHelper(
      IViewComponentHelper viewComponentHelper,
      IOptions<PackageOptionSettings> packageOptionSettings) : TagHelper
    {
        private readonly IViewComponentHelper _viewComponentHelper = viewComponentHelper;
        private readonly IOptions<PackageOptionSettings> _packageOptionSettings = packageOptionSettings;

        /// <summary>Gets or sets the ID of the form to render.</summary>
        [HtmlAttributeName("form-id")]
        public Guid FormId { get; set; }

        /// <summary>
        /// Gets or sets the name of the theme to use to render the form.
        /// </summary>
        [HtmlAttributeName("theme")]
        public string? Theme { get; set; }

        /// <summary>
        /// Gets or sets the ID of the record for editing (used only if editable records are enabled in configuration).
        /// </summary>
        [HtmlAttributeName("record-id")]
        public Guid? RecordId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether you want to exclude rendering the associated scripts with a form.
        /// </summary>
        [HtmlAttributeName("exclude-scripts")]
        public bool ExcludeScripts { get; set; }

        /// <summary>
        /// Gets or sets the Node ID of the custom redirect page after submitting a form.
        /// </summary>
        [HtmlAttributeName("redirect-to-page-id")]
        public Guid? RedirectToPageId { get; set; }

        /// <summary>
        /// Gets or sets optional additional data that will be made available to workflows.
        /// </summary>
        [HtmlAttributeName("additional-data")]
        public IDictionary<string, string?>? AdditionalData { get; set; }

        /// <summary>Gets or sets the view context.</summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; } = new ViewContext();

        /// <inheritdoc />
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            ((IViewContextAware)_viewComponentHelper).Contextualize(ViewContext);
            output.SuppressOutput();
            Guid? nullable = RecordId;
            if (!nullable.HasValue && _packageOptionSettings.Value.AllowEditableFormSubmissions && ViewContext.HttpContext.Request.Query.TryGetValue("recordId", out StringValues stringValues) && Guid.TryParse(stringValues.ToString(), out Guid result))
                nullable = new Guid?(result);
            output.PostElement.SetHtmlContent(await _viewComponentHelper.InvokeAsync(typeof(RenderFormViewComponent), new
            {
                formId = FormId,
                recordId = nullable,
                theme = Theme,
                includeScripts = !ExcludeScripts,
                redirectToPageId = RedirectToPageId,
                additionalData = AdditionalData
            }));
        }
    }
}