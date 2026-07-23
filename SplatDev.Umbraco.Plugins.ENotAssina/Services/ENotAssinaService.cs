using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SplatDev.Umbraco.Plugins.ENotAssina.Models;

namespace SplatDev.Umbraco.Plugins.ENotAssina.Services;

/// <summary>
/// Integrates with the e-Not Assina REST API (https://assinatura.e-notariado.org.br).
/// Auth is handled by <see cref="ENotAssinaAuthHandler"/>; no header setup needed here.
/// </summary>
public class ENotAssinaService(
    IHttpClientFactory httpClientFactory,
    IOptions<ENotAssinaOptions> options,
    ILogger<ENotAssinaService> logger) : IENotAssinaService
{
    private readonly ENotAssinaOptions _opts = options.Value;
    private HttpClient Http => httpClientFactory.CreateClient(ENotAssinaDefaults.HttpClientName);

    // ── Public API ────────────────────────────────────────────────────────────

    public async Task<(string DocId, string StatusUrl)> CreateDocumentAsync(
        string folderName,
        string fileName,
        string signerName,
        string signerCpf,
        string signerEmail,
        byte[] pdfBytes,
        CancellationToken ct = default)
    {
        // Step 1: Upload PDF
        var uploadId = await UploadPdfAsync(pdfBytes, fileName, ct);

        // Step 2: Create signature flow
        var payload = new
        {
            newFolderName = folderName,
            files = new[]
            {
                new
                {
                    id          = uploadId,
                    name        = fileName,
                    contentType = "application/pdf",
                }
            },
            flowActions = new[]
            {
                new
                {
                    name       = signerName,
                    identifier = signerCpf,
                    email      = signerEmail,
                    step       = 1,
                    type       = "Signatory",
                }
            },
            paymentType = _opts.PaymentType,
        };

        var response = await Http.PostAsJsonAsync("/api/documents/e-not-assina", payload, ct);
        await EnsureSuccessAsync(response, "create document", ct);

        var result = await response.Content.ReadFromJsonAsync<ENotCreateResponse>(cancellationToken: ct)
                     ?? throw new InvalidOperationException("Empty response from e-Not Assina.");

        var statusUrl = $"https://assinatura.e-notariado.org.br/documentos/{result.DocumentId}";
        return (result.DocumentId, statusUrl);
    }

    public async Task<bool> IsSignedAsync(string docId, CancellationToken ct = default)
    {
        try
        {
            var response = await Http.GetAsync($"/api/documents/{docId}", ct);
            if (!response.IsSuccessStatusCode) return false;
            var result = await response.Content.ReadFromJsonAsync<ENotStatusResponse>(cancellationToken: ct);
            return result?.Status == "Concluded";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "e-Not Assina status check failed for {DocId}", docId);
            return false;
        }
    }

    public async Task<byte[]?> DownloadSignedPdfAsync(string docId, CancellationToken ct = default)
    {
        try
        {
            var response = await Http.GetAsync($"/api/documents/{docId}/ticket", ct);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadAsByteArrayAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "e-Not Assina PDF download failed for {DocId}", docId);
            return null;
        }
    }

    public async Task CancelAsync(string docId, CancellationToken ct = default)
    {
        var response = await Http.PutAsync($"/api/documents/{docId}/canceled", null, ct);
        await EnsureSuccessAsync(response, "cancel document", ct);
    }

    public Task ProcessWebhookAsync(ENotWebhookPayload payload, CancellationToken ct = default)
    {
        logger.LogInformation(
            "e-Not Assina webhook received: event={Event}, docId={DocId}",
            payload.Event, payload.DocumentId);
        return Task.CompletedTask;
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private async Task<string> UploadPdfAsync(byte[] pdfBytes, string fileName, CancellationToken ct)
    {
        using var content     = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(pdfBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        content.Add(fileContent, "File", fileName);

        var response = await Http.PostAsync("/api/uploads", content, ct);
        await EnsureSuccessAsync(response, "upload PDF", ct);

        var result = await response.Content.ReadFromJsonAsync<ENotUploadResponse>(cancellationToken: ct)
                     ?? throw new InvalidOperationException("Empty upload response from e-Not Assina.");
        return result.Id;
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, string op, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode) return;
        var body = await response.Content.ReadAsStringAsync(ct);
        throw new HttpRequestException(
            $"e-Not Assina {op} failed HTTP {(int)response.StatusCode}. Body: {body}",
            null, response.StatusCode);
    }

    // Internal response DTOs
    private sealed class ENotUploadResponse
    {
        [JsonPropertyName("id")] public string Id { get; init; } = "";
    }

    private sealed class ENotCreateResponse
    {
        [JsonPropertyName("documentId")] public string DocumentId { get; init; } = "";
    }

    private sealed class ENotStatusResponse
    {
        [JsonPropertyName("status")] public string Status { get; init; } = "";
    }
}
