using SplatDev.Umbraco.Plugins.CustomLogin.Models;
using SplatDev.Umbraco.Plugins.CustomLogin.Services;
using Xunit;

namespace SplatDev.Umbraco.Plugins.CustomLogin.Tests;

public class LoginPageCssGeneratorTests
{
    [Fact]
    public void Generate_DefaultSettings_ContainsRootSelector()
    {
        var settings = new CustomLoginSettings();

        var css = LoginPageCssGenerator.Generate(settings);

        Assert.Contains(":root {", css);
        Assert.Contains("}", css);
    }

    [Fact]
    public void Generate_WithBackgroundColor_EmitsCustomProperty()
    {
        var settings = new CustomLoginSettings { BackgroundColor = "#1a1a2e" };

        var css = LoginPageCssGenerator.Generate(settings);

        Assert.Contains("--umb-login-background: #1a1a2e;", css);
    }

    [Fact]
    public void Generate_WithPrimaryAndTextColors_EmitsBothProperties()
    {
        var settings = new CustomLoginSettings
        {
            PrimaryColor = "#e94560",
            TextColor = "#ffffff",
        };

        var css = LoginPageCssGenerator.Generate(settings);

        Assert.Contains("--umb-login-primary-color: #e94560;", css);
        Assert.Contains("--umb-login-text-color: #ffffff;", css);
    }

    [Fact]
    public void Generate_ShowCurvesFalse_HidesCurves()
    {
        var settings = new CustomLoginSettings { ShowCurves = false };

        var css = LoginPageCssGenerator.Generate(settings);

        Assert.Contains("--umb-login-curves-display: none;", css);
    }

    [Fact]
    public void Generate_ShowCurvesTrue_DoesNotHideCurves()
    {
        var settings = new CustomLoginSettings { ShowCurves = true };

        var css = LoginPageCssGenerator.Generate(settings);

        Assert.DoesNotContain("--umb-login-curves-display: none;", css);
    }

    [Fact]
    public void Generate_ShowImagePanelFalse_HidesImage()
    {
        var settings = new CustomLoginSettings { ShowImagePanel = false };

        var css = LoginPageCssGenerator.Generate(settings);

        Assert.Contains("--umb-login-image-display: none;", css);
    }

    [Fact]
    public void Generate_WithBackgroundImageUrl_EmitsUrlProperty()
    {
        var settings = new CustomLoginSettings { BackgroundImageUrl = "/media/bg.jpg" };

        var css = LoginPageCssGenerator.Generate(settings);

        Assert.Contains("--umb-login-image: url('/media/bg.jpg');", css);
    }

    [Fact]
    public void Generate_WithLayoutProperties_EmitsAllLayoutProperties()
    {
        var settings = new CustomLoginSettings
        {
            ImageBorderRadius = "12px",
            ContentBackground = "rgba(0,0,0,0.8)",
            ContentWidth = "400px",
            ContentHeight = "500px",
            ContentBorderRadius = "8px",
            AlignItems = "flex-start",
        };

        var css = LoginPageCssGenerator.Generate(settings);

        Assert.Contains("--umb-login-image-border-radius: 12px;", css);
        Assert.Contains("--umb-login-content-background: rgba(0,0,0,0.8);", css);
        Assert.Contains("--umb-login-content-width: 400px;", css);
        Assert.Contains("--umb-login-content-height: 500px;", css);
        Assert.Contains("--umb-login-content-border-radius: 8px;", css);
        Assert.Contains("--umb-login-align-items: flex-start;", css);
    }

    [Fact]
    public void Generate_WithTypographyAndButtons_EmitsProperties()
    {
        var settings = new CustomLoginSettings
        {
            HeaderFontSize = "1.5rem",
            HeaderFontSizeLarge = "2rem",
            HeaderSecondaryFontSize = "1rem",
            ButtonBorderRadius = "4px",
        };

        var css = LoginPageCssGenerator.Generate(settings);

        Assert.Contains("--umb-login-header-font-size: 1.5rem;", css);
        Assert.Contains("--umb-login-header-font-size-large: 2rem;", css);
        Assert.Contains("--umb-login-header-secondary-font-size: 1rem;", css);
        Assert.Contains("--umb-login-button-border-radius: 4px;", css);
    }

    [Fact]
    public void Generate_WithTimeoutImage_EmitsSessionExpiredOverride()
    {
        var settings = new CustomLoginSettings { TimeoutBackgroundImageUrl = "/media/timeout.jpg" };

        var css = LoginPageCssGenerator.Generate(settings);

        Assert.Contains("umb-auth-layout[flow-is-not='login']", css);
        Assert.Contains("url('/media/timeout.jpg')", css);
    }

    [Fact]
    public void Generate_WithCustomCss_AppendsRawCss()
    {
        var settings = new CustomLoginSettings { CustomCss = ".my-class { color: red; }" };

        var css = LoginPageCssGenerator.Generate(settings);

        Assert.Contains("/* Custom CSS overrides */", css);
        Assert.Contains(".my-class { color: red; }", css);
    }

    [Fact]
    public void Generate_EmptyStringProperties_OmitsThoseProperties()
    {
        var settings = new CustomLoginSettings();

        var css = LoginPageCssGenerator.Generate(settings);

        Assert.DoesNotContain("--umb-login-background:", css);
        Assert.DoesNotContain("--umb-login-primary-color:", css);
        Assert.DoesNotContain("--umb-login-text-color:", css);
    }

    [Fact]
    public void Generate_WithFooterText_EmitsFooterOverlay()
    {
        var settings = new CustomLoginSettings { FooterText = "© 2026 SplatDev" };

        var css = LoginPageCssGenerator.Generate(settings);

        Assert.Contains("umb-auth-layout::after", css);
        Assert.Contains("© 2026 SplatDev", css);
        Assert.Contains("position: fixed", css);
    }

    [Fact]
    public void Generate_WithoutFooterText_OmitsFooterOverlay()
    {
        var settings = new CustomLoginSettings();

        var css = LoginPageCssGenerator.Generate(settings);

        Assert.DoesNotContain("umb-auth-layout::after", css);
    }
}
