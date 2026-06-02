namespace SplatDev.Umbraco.Plugins.CustomLogin.Models;

public class CustomLoginSettings
{
    // -- Branding --
    public string BrandName { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string LogoAlternativeUrl { get; set; } = string.Empty;
    public string BackgroundImageUrl { get; set; } = string.Empty;
    public string SupportEmail { get; set; } = string.Empty;
    public string LoginPageTitle { get; set; } = string.Empty;
    public string FaviconUrl { get; set; } = string.Empty;
    public string FooterText { get; set; } = string.Empty;

    // -- Colors (map to Umbraco CSS custom properties) --
    public string BackgroundColor { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = string.Empty;
    public string TextColor { get; set; } = string.Empty;
    public string CurvesColor { get; set; } = string.Empty;
    public bool ShowCurves { get; set; } = true;

    // -- Layout --
    public bool ShowImagePanel { get; set; } = true;
    public string ImageBorderRadius { get; set; } = string.Empty;
    public string ContentBackground { get; set; } = string.Empty;
    public string ContentWidth { get; set; } = string.Empty;
    public string ContentHeight { get; set; } = string.Empty;
    public string ContentBorderRadius { get; set; } = string.Empty;
    public string AlignItems { get; set; } = string.Empty;

    // -- Typography --
    public string HeaderFontSize { get; set; } = string.Empty;
    public string HeaderFontSizeLarge { get; set; } = string.Empty;
    public string HeaderSecondaryFontSize { get; set; } = string.Empty;

    // -- Buttons --
    public string ButtonBorderRadius { get; set; } = string.Empty;

    // -- Security --
    public bool AllowPasswordReset { get; set; } = true;
    public bool EnableSso { get; set; }

    // -- Greetings (day of week: 0 = Sunday, 6 = Saturday) --
    public string[] Greetings { get; set; } = new string[7];
    public string[] GreetingsEs { get; set; } = new string[7];

    // -- Timeout / Session Expired --
    public string TimeoutBackgroundImageUrl { get; set; } = string.Empty;
    public string TimeoutInstructionText { get; set; } = string.Empty;
    public string TimeoutInstructionTextEs { get; set; } = string.Empty;

    // -- Custom CSS (raw override appended after generated styles) --
    public string CustomCss { get; set; } = string.Empty;
}
