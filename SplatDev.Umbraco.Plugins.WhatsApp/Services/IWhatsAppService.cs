using SplatDev.Umbraco.Plugins.WhatsApp.Models;

namespace SplatDev.Umbraco.Plugins.WhatsApp.Services;

public interface IWhatsAppService
{
    WhatsAppSettings GetSettings();
    string GenerateWhatsAppUrl(string phoneNumber, string welcomeMessage);
}
