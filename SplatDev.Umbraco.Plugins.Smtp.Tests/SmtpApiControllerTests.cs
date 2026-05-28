using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using SplatDev.Umbraco.Plugins.Smtp.Controllers;
using SplatDev.Umbraco.Plugins.Smtp.Models;
using SplatDev.Umbraco.Plugins.Smtp.Services;

namespace SplatDev.Umbraco.Plugins.Smtp.Tests;

public class SmtpApiControllerTests
{
    private readonly Mock<ISmtpService> _service;
    private readonly SmtpApiController _sut;

    public SmtpApiControllerTests()
    {
        _service = new Mock<ISmtpService>();
        _sut = new SmtpApiController(_service.Object);
    }

    [Fact]
    public void GetSettings_ReturnsOkWithMaskedPassword()
    {
        _service.Setup(s => s.GetSettings()).Returns(new SmtpSettings
        {
            Host = "smtp.example.com", Port = 587, Username = "user",
            Password = "secret123", EnableSsl = true,
            FromEmail = "noreply@example.com", FromName = "Test"
        });

        var result = _sut.GetSettings();

        var ok = Assert.IsType<OkObjectResult>(result);
        var settings = Assert.IsType<SmtpSettings>(ok.Value);
        Assert.Equal("********", settings.Password);
        Assert.Equal("smtp.example.com", settings.Host);
    }

    [Fact]
    public async Task TestConnection_Valid_ReturnsOk()
    {
        var settings = new SmtpSettings { Host = "smtp.example.com", Port = 587 };
        _service.Setup(s => s.TestConnectionAsync(settings))
            .ReturnsAsync(new SmtpTestResult { Success = true, Message = "Connected" });

        var result = await _sut.TestConnection(settings);

        Assert.IsType<OkObjectResult>(result);
    }
}
