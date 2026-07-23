using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SplatDev.Umbraco.Plugins.D4Sign.Models;

namespace SplatDev.Umbraco.Plugins.D4Sign.Services;

/// <summary>
/// Integrates with the D4Sign REST API (https://secure.d4sign.com.br/api/v1).
/// Auth credentials (tokenAPI + cryptKey) are appended to every request by
/// <see cref="D4SignAuthHandler"/> — no extra header configuration needed here.
/// </summary>
public class D4SignService(
    IHttpClientFactory httpClientFactory,
    IOptions<D4SignOptions> options,
    ILogger<D4SignService> logger) : ID4SignService
{
    private readonly D4SignOptions _opts = options.Value;
    private HttpClient Http => httpClientFactory.CreateClient(D4SignDefaults.HttpClientName);

    // ── Public API ────────────────────────────────────────────────────────────

    public async Task<(string DocUuid, string StatusUrl)> CreateDocumentAsync(
        string razaoSocial,
        string signerName,
        string signerEmail,
        byte[] pdfBytes,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(_opts.SafeUuid, nameof(_opts.SafeUuid));

        var fileName = $"Contrato_{SanitizeFileName(razaoSocial)}.pdf";
        var docUuid  = await UploadPdfAsync(pdfBytes, fileName, ct);

        await AddSignerAsync(docUuid, signerEmail, signerName, ct);
        await SendToSignersAsync(docUuid, razaoSocial, ct);

        if (!string.IsNullOrWhiteSpace(_opts.WebhookUrl))
        {
            try
            {
                await RegisterWebhookAsync(docUuid, _opts.WebhookUrl, ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex,
                    "D4Sign webhook registration failed for doc {DocUuid}. Proceeding without webhook.", docUuid);
            }
        }

        var statusUrl = $"https://secure.d4sign.com.br/documentos/{docUuid}";
        return (docUuid, statusUrl);
    }

    public async Task<bool> IsSignedAsync(string docUuid, CancellationToken ct = default)
    {
        try
        {
            var response = await Http.GetAsync($"/api/v1/documents/{docUuid}", ct);
            if (!response.IsSuccessStatusCode) return false;

            var info = await response.Content.ReadFromJsonAsync<D4SignDocumentInfo>(cancellationToken: ct);
            return info?.StatusId == "3";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "D4Sign status check failed for {DocUuid}", docUuid);
            return false;
        }
    }

    public async Task<byte[]?> DownloadSignedPdfAsync(string docUuid, CancellationToken ct = default)
    {
        try
        {
            var response = await Http.GetAsync($"/api/v1/documents/{docUuid}/download", ct);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadAsByteArrayAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "D4Sign PDF download failed for {DocUuid}", docUuid);
            return null;
        }
    }

    public Task ProcessWebhookAsync(D4SignWebhookPayload payload, CancellationToken ct = default)
    {
        // Base implementation logs the event. Host applications should override
        // or subscribe to the signed event to add their own business logic
        // (e.g. activating a lease, sending a welcome e-mail, etc.).
        logger.LogInformation(
            "D4Sign webhook received: type={TypePost}, uuid={Uuid}",
            payload.TypePost, payload.UuidDocument);
        return Task.CompletedTask;
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private async Task<string> UploadPdfAsync(byte[] pdfBytes, string fileName, CancellationToken ct)
    {
        using var content     = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(pdfBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        content.Add(fileContent, "file", fileName);

        var response = await Http.PostAsync($"/api/v1/documents/{_opts.SafeUuid}/upload", content, ct);
        var body     = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(
                $"D4Sign upload failed HTTP {(int)response.StatusCode}. Body: {body}",
                null, response.StatusCode);

        if (string.IsNullOrWhiteSpace(body))
            throw new InvalidOperationException("D4Sign upload returned an empty response.");

        D4SignUploadResponse? result;
        try { result = JsonSerializer.Deserialize<D4SignUploadResponse>(body); }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"D4Sign upload returned non-JSON: {body}", ex);
        }

        if (result is null || string.IsNullOrWhiteSpace(result.UuidDocument))
            throw new InvalidOperationException($"D4Sign upload missing uuid_document. Body: {body}");

        return result.UuidDocument;
    }

    private async Task AddSignerAsync(string docUuid, string email, string name, CancellationToken ct)
    {
        var payload = new
        {
            signers = new[]
            {
                new
                {
                    email,
                    act              = "1",   // 1 = signer
                    foreign          = "0",
                    certificadoicpbr = "0",
                    assinar_como_pj  = "0",
                },
            },
        };

        var response = await Http.PostAsJsonAsync($"/api/v1/documents/{docUuid}/createlist", payload, ct);
        await EnsureSuccessAsync(response, "add signer", ct);
    }

    private async Task SendToSignersAsync(string docUuid, string razaoSocial, CancellationToken ct)
    {
        var payload = new
        {
            message    = $"Por favor, assine o contrato de {razaoSocial}.",
            skip_email = "0",
        };

        var response = await Http.PostAsJsonAsync($"/api/v1/documents/{docUuid}/sendtosigner", payload, ct);
        await EnsureSuccessAsync(response, "send to signer", ct);
    }

    private async Task RegisterWebhookAsync(string docUuid, string webhookUrl, CancellationToken ct)
    {
        var payload  = new { url = webhookUrl, type = "Redirect" };
        var response = await Http.PostAsJsonAsync($"/api/v1/documents/{docUuid}/webhooks", payload, ct);
        await EnsureSuccessAsync(response, "register webhook", ct);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, string op, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode) return;
        var body = await response.Content.ReadAsStringAsync(ct);
        throw new HttpRequestException(
            $"D4Sign {op} failed HTTP {(int)response.StatusCode}. Body: {body}",
            null, response.StatusCode);
    }

    private static string SanitizeFileName(string name) =>
        string.Concat(name.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c))
              .Replace(' ', '_')[..Math.Min(name.Length, 50)];

    // Internal DTOs
    private sealed class D4SignUploadResponse
    {
        [JsonPropertyName("uuid_document")]
        public string UuidDocument { get; set; } = "";
    }

    private sealed class D4SignDocumentInfo
    {
        [JsonPropertyName("statusId")]
        public string StatusId { get; set; } = "";
    }
}
