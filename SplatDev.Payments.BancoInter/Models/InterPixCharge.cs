using System.Text.Json.Serialization;

namespace SplatDev.Payments.BancoInter.Models;

/// <summary>Request body for creating a Pix immediate charge (cob).</summary>
public class InterPixChargeRequest
{
    [JsonPropertyName("calendario")]
    public InterCalendario Calendario { get; set; } = new();

    [JsonPropertyName("devedor")]
    public InterDevedor? Devedor { get; set; }

    [JsonPropertyName("valor")]
    public InterValor Valor { get; set; } = new();

    [JsonPropertyName("chave")]
    public string Chave { get; set; } = string.Empty;

    [JsonPropertyName("solicitacaoPagador")]
    public string? SolicitacaoPagador { get; set; }

    [JsonPropertyName("infoAdicionais")]
    public List<InterInfoAdicional>? InfoAdicionais { get; set; }
}

public class InterCalendario
{
    /// <summary>Expiration in seconds from charge creation.</summary>
    [JsonPropertyName("expiracao")]
    public int Expiracao { get; set; } = 3600;

    /// <summary>Due date for cobv charges (yyyy-MM-dd).</summary>
    [JsonPropertyName("dataDeVencimento")]
    public string? DataDeVencimento { get; set; }

    /// <summary>Number of days after due date that the charge remains payable.</summary>
    [JsonPropertyName("validadeAposVencimento")]
    public int? ValidadeAposVencimento { get; set; }
}

public class InterDevedor
{
    [JsonPropertyName("cpf")]
    public string? Cpf { get; set; }

    [JsonPropertyName("cnpj")]
    public string? Cnpj { get; set; }

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;
}

public class InterValor
{
    [JsonPropertyName("original")]
    public string Original { get; set; } = "0.00";

    [JsonPropertyName("modalidadeAlteracao")]
    public int? ModalidadeAlteracao { get; set; }

    [JsonPropertyName("desconto")]
    public InterDesconto? Desconto { get; set; }

    [JsonPropertyName("juros")]
    public InterJuros? Juros { get; set; }

    [JsonPropertyName("multa")]
    public InterMulta? Multa { get; set; }

    [JsonPropertyName("abatimento")]
    public InterAbatimento? Abatimento { get; set; }
}

public class InterDesconto
{
    [JsonPropertyName("modalidade")]
    public int Modalidade { get; set; }

    [JsonPropertyName("valorPerc")]
    public string? ValorPerc { get; set; }

    [JsonPropertyName("descontoDataFixa")]
    public List<InterDescontoDataFixa>? DescontoDataFixa { get; set; }
}

public class InterDescontoDataFixa
{
    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;

    [JsonPropertyName("valorPerc")]
    public string ValorPerc { get; set; } = string.Empty;
}

public class InterJuros
{
    [JsonPropertyName("modalidade")]
    public int Modalidade { get; set; }

    [JsonPropertyName("valorPerc")]
    public string ValorPerc { get; set; } = string.Empty;
}

public class InterMulta
{
    [JsonPropertyName("modalidade")]
    public int Modalidade { get; set; }

    [JsonPropertyName("valorPerc")]
    public string ValorPerc { get; set; } = string.Empty;
}

public class InterAbatimento
{
    [JsonPropertyName("modalidade")]
    public int Modalidade { get; set; }

    [JsonPropertyName("valorPerc")]
    public string ValorPerc { get; set; } = string.Empty;
}

public class InterInfoAdicional
{
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("valor")]
    public string Valor { get; set; } = string.Empty;
}
