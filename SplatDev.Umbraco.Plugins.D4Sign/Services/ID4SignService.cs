using SplatDev.Umbraco.Plugins.D4Sign.Models;

namespace SplatDev.Umbraco.Plugins.D4Sign.Services;

/// <summary>
/// Core D4Sign operations: upload, signer management, webhook handling, and status queries.
/// </summary>
public interface ID4SignService
{
    /// <summary>
    /// Uploads a PDF to the configured D4Sign safe, adds a signer, sends the document
    /// for signature, and optionally registers a webhook callback.
    /// </summary>
    /// <returns>(docUuid, statusUrl) — persist docUuid in your own table.</returns>
    Task<(string DocUuid, string StatusUrl)> CreateDocumentAsync(
        string razaoSocial,
        string signerName,
        string signerEmail,
        byte[] pdfBytes,
        CancellationToken ct = default);

    /// <summary>Returns <c>true</c> if D4Sign reports the document as fully signed (statusId = "3").</summary>
    Task<bool> IsSignedAsync(string docUuid, CancellationToken ct = default);

    /// <summary>
    /// Downloads the signed PDF bytes from D4Sign.
    /// Returns <c>null</c> when the document is not yet signed or the download fails.
    /// </summary>
    Task<byte[]?> DownloadSignedPdfAsync(string docUuid, CancellationToken ct = default);

    /// <summary>Processes an inbound D4Sign webhook payload (document_signed, document_canceled …).</summary>
    Task ProcessWebhookAsync(D4SignWebhookPayload payload, CancellationToken ct = default);
}
