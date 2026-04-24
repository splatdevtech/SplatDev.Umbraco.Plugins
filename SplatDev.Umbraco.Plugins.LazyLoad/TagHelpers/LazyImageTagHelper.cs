using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SplatDev.Umbraco.Plugins.LazyLoad.TagHelpers;

[HtmlTargetElement("img")]
public class LazyImageTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var src = output.Attributes["src"]?.Value?.ToString();
        if (!string.IsNullOrEmpty(src))
        {
            output.Attributes.SetAttribute("data-src", src);
            output.Attributes.SetAttribute("src", "data:image/gif;base64,R0lGODlhAQABAAD/ACwAAAAAAQABAAACADs=");
            var existing = output.Attributes["class"]?.Value?.ToString() ?? "";
            output.Attributes.SetAttribute("class", (existing + " lazy").Trim());
        }
    }
}
