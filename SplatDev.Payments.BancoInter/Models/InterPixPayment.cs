using System.Text.Json.Serialization;

namespace SplatDev.Payments.BancoInter.Models;

/// <summary>Request body for making an outbound Pix payment via Banking API.</summary>
public class InterPixPaymentRequest
{
    [JsonPropertyName("valor")]
    public decimal Valor { get; set; }

    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }

    [JsonPropertyName("destinatario")]
    public InterPixDestinatario Destinatario { get; set; } = new();
}

public class InterPixDestinatario
{
    [JsonPropertyName("tipo")]
    public string Tipo { get; set; } = "CHAVE";

    [JsonPropertyName("chave")]
    public string? Chave { get; set; }

    [JsonPropertyName("banco")]
    public string? Banco { get; set; }

    [JsonPropertyName("agencia")]
    public string? Agencia { get; set; }

    [JsonPropertyName("conta")]
    public string? Conta { get; set; }

    [JsonPropertyName("tipoConta")]
    public string? TipoConta { get; set; }

    [JsonPropertyName("nome")]
    public string? Nome { get; set; }

    [JsonPropertyName("cpfCnpj")]
    public string? CpfCnpj { get; set; }
}

public class InterPixPaymentResponse
{
    [JsonPropertyName("codigoSolicitacao")]
    public string CodigoSolicitacao { get; set; } = string.Empty;

    [JsonPropertyName("endToEndId")]
    public string? EndToEndId { get; set; }

    [JsonPropertyName("valor")]
    public decimal Valor { get; set; }

    [JsonPropertyName("dataHoraOperacao")]
    public DateTimeOffset DataHoraOperacao { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

/// <summary>Request body for paying a boleto/convenio/tribute via Banking API.</summary>
public class InterBoletoPaymentRequest
{
    [JsonPropertyName("codigoBarras")]
    public string CodigoBarras { get; set; } = string.Empty;

    [JsonPropertyName("valorPagar")]
    public decimal ValorPagar { get; set; }

    [JsonPropertyName("dataPagamento")]
    public string DataPagamento { get; set; } = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd");

    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }
}

public class InterBoletoPaymentResponse
{
    [JsonPropertyName("codigoSolicitacao")]
    public string CodigoSolicitacao { get; set; } = string.Empty;

    [JsonPropertyName("valor")]
    public decimal Valor { get; set; }

    [JsonPropertyName("dataPagamento")]
    public string DataPagamento { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}
