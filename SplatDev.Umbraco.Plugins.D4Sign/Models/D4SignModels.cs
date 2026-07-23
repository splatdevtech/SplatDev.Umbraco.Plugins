using System.Text.Json.Serialization;

namespace SplatDev.Umbraco.Plugins.D4Sign.Models;

/// <summary>
/// Row stored in the <c>d4sign_contrato</c> table returned by the dashboard API.
/// Uses snake_case property names so NPoco's dynamic mapping works without a POCO.
/// </summary>
public class D4SignDocumentRow
{
    public int    Id          { get; set; }
    public int    LocacaoId   { get; set; }
    public string DocUuid     { get; set; } = "";
    public string SafeUuid    { get; set; } = "";
    public string Status      { get; set; } = "";
    public DateTime CriadoEm  { get; set; }
    public DateTime? AssinadoEm { get; set; }
    public string? PdfBlobUrl { get; set; }

    // Joined from host application tables — may be empty when the plugin is
    // embedded in applications that use different table names.
    public string? RazaoSocial    { get; set; }
    public string? Cnpj           { get; set; }
    public string? SlugDesejado   { get; set; }
    public string? RegionalCodigo { get; set; }
    public string? Uf             { get; set; }
}

/// <summary>Request body for the check-status endpoint.</summary>
public class D4SignCheckStatusRequest
{
    [JsonPropertyName("docUuid")]
    public string DocUuid { get; set; } = "";

    [JsonPropertyName("locacaoId")]
    public int LocacaoId { get; set; }
}

/// <summary>Options bag read from <c>appsettings.json</c> under key <c>D4Sign</c>.</summary>
public class D4SignOptions
{
    public const string SectionKey = "D4Sign";

    public string BaseUrl    { get; set; } = "https://secure.d4sign.com.br";
    public string TokenApi   { get; set; } = "";
    public string CryptKey   { get; set; } = "";
    public string SafeUuid   { get; set; } = "";
    public string WebhookUrl { get; set; } = "";

    /// <summary>
    /// Name of the database table that stores D4Sign contract records.
    /// Defaults to <c>d4sign_contrato</c>; override in multi-tenant hosts.
    /// </summary>
    public string TableName  { get; set; } = "d4sign_contrato";
}
