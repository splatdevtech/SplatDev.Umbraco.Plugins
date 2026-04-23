using Microsoft.Extensions.Configuration;
using UmbracoCms.Plugins.WhatsApp.Models;

namespace UmbracoCms.Plugins.WhatsApp.Services;

public class WhatsAppService : IWhatsAppService
{
    private readonly IConfiguration _configuration;

    public WhatsAppService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public WhatsAppSettings GetSettings()
    {
        var section = _configuration.GetSection("WhatsApp");
        return new WhatsAppSettings
        {
            PhoneNumber = section["PhoneNumber"] ?? string.Empty,
            WelcomeMessage = section["WelcomeMessage"] ?? string.Empty,
            ButtonPosition = section["ButtonPosition"] ?? "bottom-right",
            IsEnabled = bool.TryParse(section["IsEnabled"], out var enabled) ? enabled : true
        };
    }

    public string GenerateWhatsAppUrl(string phoneNumber, string welcomeMessage)
    {
        var sanitizedPhone = phoneNumber.Replace("+", "").Replace(" ", "").Replace("-", "");
        var encodedMessage = Uri.EscapeDataString(welcomeMessage);
        return $"https://wa.me/{sanitizedPhone}?text={encodedMessage}";
    }
}
