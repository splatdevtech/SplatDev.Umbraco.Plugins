using System.Text.Json.Serialization;

namespace SplatDev.Payments.BancoInter.Models;

/// <summary>Payload received from Banco Inter Pix webhooks on payment confirmation.</summary>
public class InterPixWebhookPayload
{
    [JsonPropertyName("pix")]
    public List<InterPixWebhookEvent> Pix { get; set; } = [];
}

public class InterPixWebhookEvent
{
    [JsonPropertyName("endToEndId")]
    public string EndToEndId { get; set; } = string.Empty;

    [JsonPropertyName("txid")]
    public string Txid { get; set; } = string.Empty;

    [JsonPropertyName("chave")]
    public string Chave { get; set; } = string.Empty;

    [JsonPropertyName("valor")]
    public string Valor { get; set; } = string.Empty;

    [JsonPropertyName("horario")]
    public DateTimeOffset Horario { get; set; }

    [JsonPropertyName("infoPagador")]
    public string? InfoPagador { get; set; }

    [JsonPropertyName("devolucoes")]
    public List<InterDevolucao>? Devolucoes { get; set; }
}

/// <summary>Payload for Banking (Pix outbound payment) webhook events.</summary>
public class InterBankingWebhookPayload
{
    [JsonPropertyName("tipoEvento")]
    public string TipoEvento { get; set; } = string.Empty;

    [JsonPropertyName("codigoSolicitacao")]
    public string CodigoSolicitacao { get; set; } = string.Empty;

    [JsonPropertyName("endToEndId")]
    public string? EndToEndId { get; set; }

    [JsonPropertyName("valor")]
    public decimal Valor { get; set; }

    [JsonPropertyName("dataHoraEvento")]
    public DateTimeOffset DataHoraEvento { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}
