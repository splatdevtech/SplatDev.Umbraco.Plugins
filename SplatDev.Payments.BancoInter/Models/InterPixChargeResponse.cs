using System.Text.Json.Serialization;

namespace SplatDev.Payments.BancoInter.Models;

public class InterPixChargeResponse
{
    [JsonPropertyName("txid")]
    public string Txid { get; set; } = string.Empty;

    [JsonPropertyName("revisao")]
    public int Revisao { get; set; }

    [JsonPropertyName("loc")]
    public InterLocation? Loc { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("calendario")]
    public InterCalendario? Calendario { get; set; }

    [JsonPropertyName("devedor")]
    public InterDevedor? Devedor { get; set; }

    [JsonPropertyName("valor")]
    public InterValor? Valor { get; set; }

    [JsonPropertyName("chave")]
    public string Chave { get; set; } = string.Empty;

    [JsonPropertyName("solicitacaoPagador")]
    public string? SolicitacaoPagador { get; set; }

    [JsonPropertyName("infoAdicionais")]
    public List<InterInfoAdicional>? InfoAdicionais { get; set; }

    [JsonPropertyName("pixCopiaECola")]
    public string? PixCopiaECola { get; set; }

    [JsonPropertyName("pix")]
    public List<InterPixReceived>? Pix { get; set; }
}

public class InterLocation
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty;

    [JsonPropertyName("tipoCob")]
    public string TipoCob { get; set; } = string.Empty;
}

public class InterPixReceived
{
    [JsonPropertyName("endToEndId")]
    public string EndToEndId { get; set; } = string.Empty;

    [JsonPropertyName("txid")]
    public string Txid { get; set; } = string.Empty;

    [JsonPropertyName("valor")]
    public string Valor { get; set; } = string.Empty;

    [JsonPropertyName("horario")]
    public DateTimeOffset Horario { get; set; }

    [JsonPropertyName("infoPagador")]
    public string? InfoPagador { get; set; }

    [JsonPropertyName("devolucoes")]
    public List<InterDevolucao>? Devolucoes { get; set; }
}

public class InterDevolucao
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("rtrId")]
    public string RtrId { get; set; } = string.Empty;

    [JsonPropertyName("valor")]
    public string Valor { get; set; } = string.Empty;

    [JsonPropertyName("horario")]
    public InterDevolucaoHorario? Horario { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

public class InterDevolucaoHorario
{
    [JsonPropertyName("solicitacao")]
    public DateTimeOffset Solicitacao { get; set; }

    [JsonPropertyName("liquidacao")]
    public DateTimeOffset? Liquidacao { get; set; }
}
