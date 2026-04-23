using Microsoft.AspNetCore.Razor.TagHelpers;

namespace UmbracoCms.Plugins.LazyLoad.TagHelpers;

[HtmlTargetElement("iframe")]
public class LazyIframeTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var src = output.Attributes["src"]?.Value?.ToString();
        if (!string.IsNullOrEmpty(src))
        {
            output.Attributes.SetAttribute("data-src", src);
            output.Attributes.RemoveAll("src");
            var existing = output.Attributes["class"]?.Value?.ToString() ?? "";
            output.Attributes.SetAttribute("class", (existing + " lazy").Trim());
        }
    }
}
