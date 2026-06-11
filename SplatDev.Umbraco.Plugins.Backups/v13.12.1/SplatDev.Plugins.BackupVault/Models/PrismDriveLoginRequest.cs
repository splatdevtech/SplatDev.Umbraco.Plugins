using System.Text.Json.Serialization;

namespace SplatDev.Plugins.BackupVault.Models
{
    public class PrismDriveLoginRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("device_name")]
        public string DeviceName { get; set; } = string.Empty;

        [JsonPropertyName("token_name")]
        public string TokenName { get; set; } = string.Empty;

    }
}
