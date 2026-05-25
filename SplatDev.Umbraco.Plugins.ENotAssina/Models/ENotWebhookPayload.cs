using System.Text.Json.Serialization;

namespace SplatDev.Umbraco.Plugins.ENotAssina.Models;

/// <summary>
/// Webhook payload posted by e-Not Assina on document lifecycle events.
/// Configure the callback URL in the document flow payload under <c>webhookUrl</c>.
/// </summary>
public class ENotWebhookPayload
{
    /// <summary>Known values: "DocumentConcluded", "DocumentCanceled", "DocumentSigned".</summary>
    [JsonPropertyName("event")]
    public string Event { get; set; } = "";

    [JsonPropertyName("documentId")]
    public string DocumentId { get; set; } = "";

    [JsonPropertyName("concludedAt")]
    public DateTime? ConcludedAt { get; set; }

    [JsonPropertyName("signatories")]
    public List<ENotSignatoryInfo>? Signatories { get; set; }
}

public class ENotSignatoryInfo
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = "";

    [JsonPropertyName("signedAt")]
    public DateTime? SignedAt { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = "";
}
