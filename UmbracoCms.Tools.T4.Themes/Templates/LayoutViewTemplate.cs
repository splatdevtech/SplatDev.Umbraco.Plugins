namespace UmbracoCms.Tools.T4.Themes.Templates;

public static class LayoutViewTemplate
{
    public static string Render(string name) => $$"""
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - {{name}}</title>
    <link rel="stylesheet" href="~/css/{{name.ToLowerInvariant()}}.css" />
</head>
<body>
    <header>
        <nav class="navbar">
            <a class="navbar-brand" href="/">{{name}}</a>
        </nav>
    </header>
    <main role="main">
        @RenderBody()
    </main>
    <footer>
        <p>&copy; @DateTime.Now.Year - {{name}}</p>
    </footer>
    @RenderSection("Scripts", required: false)
</body>
</html>
""";
}
