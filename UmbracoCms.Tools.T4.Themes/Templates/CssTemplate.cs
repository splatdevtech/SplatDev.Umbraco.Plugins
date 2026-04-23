namespace UmbracoCms.Tools.T4.Themes.Templates;

public static class CssTemplate
{
    public static string Render(string name) => $$"""
/* {{name}} Theme Stylesheet */

:root {
    --primary-color: #1a73e8;
    --secondary-color: #fbbc04;
    --background-color: #ffffff;
    --text-color: #202124;
    --font-family: system-ui, -apple-system, sans-serif;
}

*, *::before, *::after {
    box-sizing: border-box;
}

body {
    font-family: var(--font-family);
    color: var(--text-color);
    background-color: var(--background-color);
    margin: 0;
    padding: 0;
}

.container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 0 1rem;
}

.navbar {
    display: flex;
    align-items: center;
    padding: 1rem;
    background-color: var(--primary-color);
    color: #fff;
}

.navbar-brand {
    color: #fff;
    text-decoration: none;
    font-size: 1.25rem;
    font-weight: 600;
}

footer {
    text-align: center;
    padding: 2rem;
    color: #6c757d;
}
""";
}
