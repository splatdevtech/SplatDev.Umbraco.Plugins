namespace UmbracoCms.Tools.T4.Themes.Templates;

public static class HomeViewTemplate
{
    public static string Render(string name) => $$"""
@inherits Umbraco.Cms.Web.Common.Views.UmbracoViewPage
@{
    Layout = "~/Views/Shared/_Layout.cshtml";
    ViewData["Title"] = Model.Name;
}

<div class="container">
    <h1>@Model.Name</h1>
    @Html.Raw(Model.Value<string>("bodyText") ?? string.Empty)
</div>
""";
}
