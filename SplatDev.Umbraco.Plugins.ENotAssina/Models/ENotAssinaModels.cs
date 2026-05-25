using System.Text.Json.Serialization;

namespace SplatDev.Umbraco.Plugins.ENotAssina.Models;

/// <summary>Options read from <c>appsettings.json</c> under key <c>ENotAssina</c>.</summary>
public class ENotAssinaOptions
{
    public const string SectionKey = "ENotAssina";

    /// <summary>e-Not Assina API base URL.</summary>
    public string BaseUrl { get; set; } = "https://assinatura.e-notariado.org.br";

    /// <summary>
    /// Subscriber company identifier assigned by the notary office.
    /// Accepted as a plain number ("123") or a full value ("Empresa123") — the auth
    /// handler normalises it to the required header format automatically.
    /// </summary>
    public string CompanyId { get; set; } = "";

    /// <summary>Access key issued by the notary office.</summary>
    public string AccessKey { get; set; } = "";

    /// <summary>
    /// Payment type passed when creating a signature flow.
    /// Accepted values: "Pix" (subscriber/mensalista plan) or "Debit".
    /// </summary>
    public string PaymentType { get; set; } = "Pix";

    /// <summary>
    /// Name of the database table that stores e-Not Assina contract records.
    /// Defaults to <c>enotassina_contrato</c>; override in multi-tenant hosts.
    /// </summary>
    public string TableName { get; set; } = "enotassina_contrato";

    /// <summary>
    /// When set, the <c>Documents</c> dashboard endpoint performs a JOIN against
    /// related locação and cadastro tables to enrich the result set.
    /// Leave empty to use a plain <c>SELECT * FROM TableName</c>.
    /// </summary>
    public string LocacaoTableName { get; set; } = "";

    /// <summary>Table that links a locação to a cadastro único (e.g. risin_cadastro_locacao).</summary>
    public string CadastroLocacaoTableName { get; set; } = "";

    /// <summary>Table that holds company/member data (e.g. risin_cadastro_unico).</summary>
    public string CadastroUnicoTableName { get; set; } = "";
}

/// <summary>Request body for the check-status backoffice endpoint.</summary>
public class ENotCheckStatusRequest
{
    [JsonPropertyName("docId")]
    public string DocId { get; set; } = "";

    [JsonPropertyName("locacaoId")]
    public int LocacaoId { get; set; }
}

/// <summary>Request body for the cancel backoffice endpoint.</summary>
public class ENotCancelRequest
{
    [JsonPropertyName("docId")]
    public string DocId { get; set; } = "";
}

/// <summary>Shared constants for the e-Not Assina plugin.</summary>
public static class ENotAssinaDefaults
{
    public const string HttpClientName = "ENotAssina";
    public const string ApiRoutePrefix = "umbraco/api/enotassina";
}
