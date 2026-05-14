using SplatDev.Umbraco.Plugins.ENotAssina.Models;

namespace SplatDev.Umbraco.Plugins.ENotAssina.Services;

/// <summary>
/// Core e-Not Assina operations: upload PDF, create signature flow, webhook handling,
/// status checking, PDF download, and flow cancellation.
/// </summary>
public interface IENotAssinaService
{
    /// <summary>
    /// Uploads a PDF, creates a single-signer signature flow, and returns
    /// (documentId, statusUrl). Persist documentId in your own table.
    /// </summary>
    Task<(string DocId, string StatusUrl)> CreateDocumentAsync(
        string folderName,
        string fileName,
        string signerName,
        string signerCpf,
        string signerEmail,
        byte[] pdfBytes,
        CancellationToken ct = default);

    /// <summary>Returns <c>true</c> if e-Not Assina reports status "Concluded".</summary>
    Task<bool> IsSignedAsync(string docId, CancellationToken ct = default);

    /// <summary>
    /// Downloads the signed PDF (ticket) from e-Not Assina.
    /// Returns <c>null</c> when the document is not concluded or download fails.
    /// </summary>
    Task<byte[]?> DownloadSignedPdfAsync(string docId, CancellationToken ct = default);

    /// <summary>Cancels a pending signature flow.</summary>
    Task CancelAsync(string docId, CancellationToken ct = default);

    /// <summary>
    /// Processes an inbound e-Not Assina webhook event.
    /// Base implementation logs the event; override to add business logic.
    /// </summary>
    Task ProcessWebhookAsync(ENotWebhookPayload payload, CancellationToken ct = default);
}
