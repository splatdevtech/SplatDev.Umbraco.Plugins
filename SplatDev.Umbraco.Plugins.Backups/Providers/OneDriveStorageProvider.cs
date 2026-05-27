namespace SplatDev.Umbraco.Plugins.Backups.Providers;

using System.Net.Http.Headers;
using System.Text.Json;
using SplatDev.Umbraco.Plugins.Backups.Configuration;
using SplatDev.Umbraco.Plugins.Backups.Models;

public class OneDriveStorageProvider : ICloudStorageProvider
{
    private readonly BackupSettings _settings;
    private static readonly HttpClient _http = new();

    private const string GraphBase = "https://graph.microsoft.com/v1.0/me/drive";

    public string ProviderName => "OneDrive";
    public string ProviderIcon => "onedrive";
    public bool RequiresOAuth => true;
    public bool RequiresApiKey => false;

    public OneDriveStorageProvider(BackupSettings settings)
    {
        _settings = settings;
    }

    private (string AccessToken, string FolderPath) GetConfig()
    {
        var cfg = _settings.CloudProviders
            .FirstOrDefault(c => c.ProviderType == ProviderName && c.Enabled);
        if (cfg is null)
            throw new InvalidOperationException("OneDrive provider not configured.");

        var token = cfg.Settings.GetValueOrDefault("AccessToken") ?? string.Empty;
        var folder = cfg.Settings.GetValueOrDefault("FolderPath") ?? "Backups";
        return (token, folder);
    }

    public async Task UploadAsync(Stream data, string remotePath, string fileName, CancellationToken ct)
    {
        var (token, rootFolder) = GetConfig();
        var fullPath = $"{rootFolder}/{remotePath}/{fileName}";
        var encodedPath = Uri.EscapeDataString(fullPath).Replace("%2F", "/");

        using var request = new HttpRequestMessage(HttpMethod.Put,
            $"{GraphBase}/root:/{encodedPath}:/content")
        {
            Content = new StreamContent(data)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

        var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<Stream> DownloadAsync(string remotePath, CancellationToken ct)
    {
        var (token, _) = GetConfig();
        string url;

        if (remotePath.StartsWith("onedrive:", StringComparison.Ordinal))
        {
            var itemId = remotePath["onedrive:".Length..];
            url = $"{GraphBase}/items/{itemId}/content";
        }
        else
        {
            var encodedPath = Uri.EscapeDataString(remotePath).Replace("%2F", "/");
            url = $"{GraphBase}/root:/{encodedPath}:/content";
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        response.EnsureSuccessStatusCode();

        var ms = new MemoryStream();
        await response.Content.CopyToAsync(ms, ct);
        ms.Position = 0;
        return ms;
    }

    public async Task DeleteAsync(string remotePath, CancellationToken ct)
    {
        var (token, _) = GetConfig();
        string url;

        if (remotePath.StartsWith("onedrive:", StringComparison.Ordinal))
        {
            var itemId = remotePath["onedrive:".Length..];
            url = $"{GraphBase}/items/{itemId}";
        }
        else
        {
            var encodedPath = Uri.EscapeDataString(remotePath).Replace("%2F", "/");
            url = $"{GraphBase}/root:/{encodedPath}:";
        }

        using var request = new HttpRequestMessage(HttpMethod.Delete, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> ValidateConnectionAsync(CancellationToken ct)
    {
        try
        {
            var (token, _) = GetConfig();
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{GraphBase}/root");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request, ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<StorageItem>> ListAsync(string remotePath, CancellationToken ct)
    {
        var (token, rootFolder) = GetConfig();
        try
        {
            var fullPath = string.IsNullOrEmpty(remotePath)
                ? rootFolder
                : $"{rootFolder}/{remotePath}";
            var encodedPath = Uri.EscapeDataString(fullPath).Replace("%2F", "/");

            using var request = new HttpRequestMessage(HttpMethod.Get,
                $"{GraphBase}/root:/{encodedPath}:/children?$select=id,name,size,lastModifiedDateTime,file");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
                return [];

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            var values = doc.RootElement.GetProperty("value");

            return values.EnumerateArray()
                .Where(v => v.TryGetProperty("file", out _))
                .Select(v => new StorageItem
                {
                    Name = v.GetProperty("name").GetString() ?? string.Empty,
                    Path = $"onedrive:{v.GetProperty("id").GetString()}",
                    SizeBytes = v.TryGetProperty("size", out var sz) ? sz.GetInt64() : 0,
                    LastModified = v.TryGetProperty("lastModifiedDateTime", out var dt)
                        && DateTime.TryParse(dt.GetString(), out var parsed) ? parsed : DateTime.MinValue,
                    IsDirectory = false
                }).ToList();
        }
        catch
        {
            return [];
        }
    }
}
