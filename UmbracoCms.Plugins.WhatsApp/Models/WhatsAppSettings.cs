namespace UmbracoCms.Plugins.WhatsApp.Models;

public class WhatsAppSettings
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string WelcomeMessage { get; set; } = string.Empty;
    public string ButtonPosition { get; set; } = "bottom-right";
    public bool IsEnabled { get; set; } = true;
}
