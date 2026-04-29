namespace SplatDev.Payments.BancoInter;

public class BancoInterSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public bool Sandbox { get; set; } = true;

    /// <summary>Path to PEM-encoded client certificate file (required for production mTLS).</summary>
    public string? CertificatePath { get; set; }

    /// <summary>Path to PEM-encoded private key file (required for production mTLS).</summary>
    public string? CertificateKeyPath { get; set; }

    public string BaseUrl => Sandbox
        ? "https://cdpj-sandbox.partners.uatinter.co"
        : "https://cdpj.partners.uatinter.co";

    public string TokenUrl => $"{BaseUrl}/oauth/v2/token";
}
