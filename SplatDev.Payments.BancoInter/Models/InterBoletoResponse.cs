using System.Text.Json.Serialization;

namespace SplatDev.Payments.BancoInter.Models;

public class InterBoletoResponse
{
    [JsonPropertyName("nossoNumero")]
    public string NossoNumero { get; set; } = string.Empty;

    [JsonPropertyName("codigoBarras")]
    public string CodigoBarras { get; set; } = string.Empty;

    [JsonPropertyName("linhaDigitavel")]
    public string LinhaDigitavel { get; set; } = string.Empty;

    [JsonPropertyName("qrCode")]
    public InterBoletoQrCode? QrCode { get; set; }

    [JsonPropertyName("valorNominal")]
    public decimal ValorNominal { get; set; }

    [JsonPropertyName("dataVencimento")]
    public string DataVencimento { get; set; } = string.Empty;

    [JsonPropertyName("seuNumero")]
    public string SeuNumero { get; set; } = string.Empty;

    [JsonPropertyName("situacao")]
    public string Situacao { get; set; } = string.Empty;

    [JsonPropertyName("pagador")]
    public InterPagador? Pagador { get; set; }
}

public class InterBoletoQrCode
{
    [JsonPropertyName("qrcode")]
    public string Qrcode { get; set; } = string.Empty;

    [JsonPropertyName("imagemQrcode")]
    public string ImagemQrcode { get; set; } = string.Empty;
}
