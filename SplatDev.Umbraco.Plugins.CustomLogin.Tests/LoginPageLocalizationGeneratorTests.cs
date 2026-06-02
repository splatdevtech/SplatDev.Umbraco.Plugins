using SplatDev.Umbraco.Plugins.CustomLogin.Models;
using SplatDev.Umbraco.Plugins.CustomLogin.Services;
using Xunit;

namespace SplatDev.Umbraco.Plugins.CustomLogin.Tests;

public class LoginPageLocalizationGeneratorTests
{
    [Fact]
    public void GenerateEnglish_DefaultSettings_ContainsDefaultGreetings()
    {
        var settings = new CustomLoginSettings();

        var js = LoginPageLocalizationGenerator.GenerateEnglish(settings);

        Assert.Contains("greeting0:", js);
        Assert.Contains("Happy Sunday", js);
        Assert.Contains("Happy Monday", js);
        Assert.Contains("Happy Saturday", js);
    }

    [Fact]
    public void GenerateSpanish_DefaultSettings_ContainsDefaultGreetings()
    {
        var settings = new CustomLoginSettings();

        var js = LoginPageLocalizationGenerator.GenerateSpanish(settings);

        Assert.Contains("Feliz domingo", js);
        Assert.Contains("Feliz lunes", js);
        Assert.Contains("Feliz sábado", js);
    }

    [Fact]
    public void GenerateEnglish_CustomGreetings_OverridesDefaults()
    {
        var settings = new CustomLoginSettings
        {
            Greetings = ["Custom Sun", "Custom Mon", "Custom Tue", "Custom Wed", "Custom Thu", "Custom Fri", "Custom Sat"],
        };

        var js = LoginPageLocalizationGenerator.GenerateEnglish(settings);

        Assert.Contains("Custom Sun", js);
        Assert.Contains("Custom Mon", js);
        Assert.DoesNotContain("Happy Sunday", js);
    }

    [Fact]
    public void GenerateSpanish_CustomGreetings_OverridesDefaults()
    {
        var settings = new CustomLoginSettings
        {
            GreetingsEs = ["Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb"],
        };

        var js = LoginPageLocalizationGenerator.GenerateSpanish(settings);

        Assert.Contains("Dom", js);
        Assert.DoesNotContain("Feliz domingo", js);
    }

    [Fact]
    public void GenerateEnglish_WithTimeoutInstruction_IncludesInstructionKey()
    {
        var settings = new CustomLoginSettings
        {
            TimeoutInstructionText = "Your session expired. Please log in again.",
        };

        var js = LoginPageLocalizationGenerator.GenerateEnglish(settings);

        Assert.Contains("instruction:", js);
        Assert.Contains("Your session expired. Please log in again.", js);
    }

    [Fact]
    public void GenerateSpanish_WithTimeoutInstruction_IncludesInstructionKey()
    {
        var settings = new CustomLoginSettings
        {
            TimeoutInstructionTextEs = "Su sesión ha expirado.",
        };

        var js = LoginPageLocalizationGenerator.GenerateSpanish(settings);

        Assert.Contains("instruction:", js);
        Assert.Contains("Su sesión ha expirado.", js);
    }

    [Fact]
    public void GenerateEnglish_WithoutTimeoutInstruction_OmitsInstructionKey()
    {
        var settings = new CustomLoginSettings();

        var js = LoginPageLocalizationGenerator.GenerateEnglish(settings);

        Assert.DoesNotContain("instruction:", js);
    }

    [Fact]
    public void GenerateEnglish_OutputIsValidJsModuleStructure()
    {
        var settings = new CustomLoginSettings();

        var js = LoginPageLocalizationGenerator.GenerateEnglish(settings);

        Assert.Contains("export default {", js);
        Assert.Contains("auth: {", js);
        Assert.Contains("};", js);
    }

    [Fact]
    public void GenerateV13Xml_DefaultSettings_ContainsXmlStructure()
    {
        var settings = new CustomLoginSettings();

        var xml = LoginPageLocalizationGenerator.GenerateV13Xml(settings);

        Assert.Contains("<?xml version=", xml);
        Assert.Contains("<language culture=\"en-US\">", xml);
        Assert.Contains("<area alias=\"login\">", xml);
        Assert.Contains("<key alias=\"greeting0\">", xml);
        Assert.Contains("Happy Sunday", xml);
    }

    [Fact]
    public void GenerateEnglish_PartialGreetingsArray_FallsBackToDefaults()
    {
        var settings = new CustomLoginSettings
        {
            Greetings = ["Custom Sunday", "", "", "", "", "", ""],
        };

        var js = LoginPageLocalizationGenerator.GenerateEnglish(settings);

        Assert.Contains("Custom Sunday", js);
        Assert.Contains("Happy Monday", js);
    }
}
