using System.Text.Json.Serialization;

namespace SplatDev.Payments.BancoInter.Models;

public class InterBankingBalance
{
    [JsonPropertyName("disponivel")]
    public decimal Disponivel { get; set; }

    [JsonPropertyName("bloqueadoCheque")]
    public decimal? BloqueadoCheque { get; set; }

    [JsonPropertyName("bloqueadoJudicialmente")]
    public decimal? BloqueadoJudicialmente { get; set; }

    [JsonPropertyName("bloqueadoAdministrativo")]
    public decimal? BloqueadoAdministrativo { get; set; }

    [JsonPropertyName("limite")]
    public decimal? Limite { get; set; }
}

public class InterBankingStatement
{
    [JsonPropertyName("transacoes")]
    public List<InterBankingTransaction> Transacoes { get; set; } = [];

    [JsonPropertyName("quantidadeTransacoes")]
    public int QuantidadeTransacoes { get; set; }

    [JsonPropertyName("totalEntradas")]
    public decimal TotalEntradas { get; set; }

    [JsonPropertyName("totalSaidas")]
    public decimal TotalSaidas { get; set; }
}

public class InterBankingTransaction
{
    [JsonPropertyName("dataEntrada")]
    public string DataEntrada { get; set; } = string.Empty;

    [JsonPropertyName("tipo")]
    public string Tipo { get; set; } = string.Empty;

    [JsonPropertyName("tipoDetalhe")]
    public string TipoDetalhe { get; set; } = string.Empty;

    [JsonPropertyName("titulo")]
    public string Titulo { get; set; } = string.Empty;

    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }

    [JsonPropertyName("valor")]
    public decimal Valor { get; set; }

    [JsonPropertyName("operacao")]
    public string Operacao { get; set; } = string.Empty;
}
