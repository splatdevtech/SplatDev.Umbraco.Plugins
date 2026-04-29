using System.Text.Json.Serialization;

namespace SplatDev.Payments.BancoInter.Models;

/// <summary>Request body for issuing a Boleto com Pix (cobrança).</summary>
public class InterBoletoRequest
{
    [JsonPropertyName("pagador")]
    public InterPagador Pagador { get; set; } = new();

    [JsonPropertyName("mensagem")]
    public InterMensagem? Mensagem { get; set; }

    [JsonPropertyName("dataEmissao")]
    public string DataEmissao { get; set; } = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd");

    [JsonPropertyName("dataVencimento")]
    public string DataVencimento { get; set; } = string.Empty;

    [JsonPropertyName("seuNumero")]
    public string SeuNumero { get; set; } = string.Empty;

    [JsonPropertyName("valorNominal")]
    public decimal ValorNominal { get; set; }

    [JsonPropertyName("desconto")]
    public InterBoletoDesconto? Desconto { get; set; }

    [JsonPropertyName("multa")]
    public InterBoletoMulta? Multa { get; set; }

    [JsonPropertyName("mora")]
    public InterBoletoMora? Mora { get; set; }
}

public class InterPagador
{
    [JsonPropertyName("cpfCnpj")]
    public string CpfCnpj { get; set; } = string.Empty;

    [JsonPropertyName("tipoPessoa")]
    public string TipoPessoa { get; set; } = "FISICA";

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("endereco")]
    public string Endereco { get; set; } = string.Empty;

    [JsonPropertyName("cidade")]
    public string Cidade { get; set; } = string.Empty;

    [JsonPropertyName("uf")]
    public string Uf { get; set; } = string.Empty;

    [JsonPropertyName("cep")]
    public string Cep { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("ddd")]
    public string? Ddd { get; set; }

    [JsonPropertyName("telefone")]
    public string? Telefone { get; set; }
}

public class InterMensagem
{
    [JsonPropertyName("linha1")]
    public string? Linha1 { get; set; }

    [JsonPropertyName("linha2")]
    public string? Linha2 { get; set; }

    [JsonPropertyName("linha3")]
    public string? Linha3 { get; set; }

    [JsonPropertyName("linha4")]
    public string? Linha4 { get; set; }

    [JsonPropertyName("linha5")]
    public string? Linha5 { get; set; }
}

public class InterBoletoDesconto
{
    [JsonPropertyName("codigoDesconto")]
    public string CodigoDesconto { get; set; } = "NAOTEMDESCONTO";

    [JsonPropertyName("taxa")]
    public decimal? Taxa { get; set; }

    [JsonPropertyName("valor")]
    public decimal? Valor { get; set; }

    [JsonPropertyName("quantidadeDias")]
    public int? QuantidadeDias { get; set; }
}

public class InterBoletoMulta
{
    [JsonPropertyName("codigoMulta")]
    public string CodigoMulta { get; set; } = "NAOTEMMULTA";

    [JsonPropertyName("taxa")]
    public decimal? Taxa { get; set; }

    [JsonPropertyName("valor")]
    public decimal? Valor { get; set; }
}

public class InterBoletoMora
{
    [JsonPropertyName("codigoMora")]
    public string CodigoMora { get; set; } = "ISENTO";

    [JsonPropertyName("taxa")]
    public decimal? Taxa { get; set; }

    [JsonPropertyName("valor")]
    public decimal? Valor { get; set; }
}
