using System.Text.Json.Serialization;

namespace SplatDev.Payments.BancoInter.Models;

public class InterTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = "Bearer";

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("scope")]
    public string Scope { get; set; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; set; }
}
