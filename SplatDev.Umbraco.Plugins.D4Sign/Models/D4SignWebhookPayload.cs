using System.Text.Json.Serialization;

namespace SplatDev.Umbraco.Plugins.D4Sign.Models;

/// <summary>
/// Webhook payload posted by D4Sign on every document lifecycle event.
/// Configure the webhook URL in D4Sign's portal under the document settings.
/// </summary>
public class D4SignWebhookPayload
{
    [JsonPropertyName("uuid_document")]
    public string UuidDocument { get; set; } = "";

    /// <summary>
    /// Event type. Known values: "document_signed", "document_canceled",
    /// "document_expired", "document_viewed".
    /// </summary>
    [JsonPropertyName("type_post")]
    public string TypePost { get; set; } = "";

    [JsonPropertyName("status")]
    public D4SignStatusInfo? Status { get; set; }

    [JsonPropertyName("signatures")]
    public List<D4SignSignatureInfo>? Signatures { get; set; }
}

public class D4SignStatusInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    /// <summary>D4Sign status id. "3" = Finalizado (all signed).</summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";
}

public class D4SignSignatureInfo
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = "";

    [JsonPropertyName("signed_at")]
    public string? SignedAt { get; set; }
}
