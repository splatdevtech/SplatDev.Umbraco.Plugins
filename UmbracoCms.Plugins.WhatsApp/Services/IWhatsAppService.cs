using UmbracoCms.Plugins.WhatsApp.Models;

namespace UmbracoCms.Plugins.WhatsApp.Services;

public interface IWhatsAppService
{
    WhatsAppSettings GetSettings();
    string GenerateWhatsAppUrl(string phoneNumber, string welcomeMessage);
}
