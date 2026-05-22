namespace SplatDev.Umbraco.Plugins.Backups.Providers;

using System.Net.Http.Headers;
using System.Text.Json;
using SplatDev.Umbraco.Plugins.Backups.Configuration;
using SplatDev.Umbraco.Plugins.Backups.Models;

public class BoxStorageProvider : ICloudStorageProvider
{
    private readonly BackupSettings _settings;
    private static readonly HttpClient _http = new();

    private const string ApiBase = "https://api.box.com/2.0";
    private const string UploadBase = "https://upload.box.com/api/2.0";

    public string ProviderName => "BoxNet";
    public string ProviderIcon => "box";
    public bool RequiresOAuth => true;
    public bool RequiresApiKey => false;

    public BoxStorageProvider(BackupSettings settings)
    {
        _settings = settings;
    }

    private (string AccessToken, string FolderId) GetConfig()
    {
        var cfg = _settings.CloudProviders
            .FirstOrDefault(c => c.ProviderType == ProviderName && c.Enabled);
        if (cfg is null)
            throw new InvalidOperationException("Box.net provider not configured.");

        var token = cfg.Settings.GetValueOrDefault("AccessToken") ?? string.Empty;
        var folderId = cfg.Settings.GetValueOrDefault("FolderId") ?? "0";
        return (token, folderId);
    }

    public async Task UploadAsync(Stream data, string remotePath, string fileName, CancellationToken ct)
    {
        var (token, rootFolderId) = GetConfig();
        var folderId = await EnsureFolderPathAsync(token, remotePath, rootFolderId, ct);

        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(JsonSerializer.Serialize(new
        {
            name = fileName,
            parent = new { id = folderId }
        })), "attributes");

        var streamContent = new StreamContent(data);
        streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
        content.Add(streamContent, "file", fileName);

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{UploadBase}/files/content")
        {
            Content = content
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<Stream> DownloadAsync(string remotePath, CancellationToken ct)
    {
        var (token, _) = GetConfig();
        var fileId = remotePath.StartsWith("box:", StringComparison.Ordinal)
            ? remotePath["box:".Length..]
            : remotePath;

        using var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiBase}/files/{fileId}/content");
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
        var fileId = remotePath.StartsWith("box:", StringComparison.Ordinal)
            ? remotePath["box:".Length..]
            : remotePath;

        using var request = new HttpRequestMessage(HttpMethod.Delete, $"{ApiBase}/files/{fileId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> ValidateConnectionAsync(CancellationToken ct)
    {
        try
        {
            var (token, folderId) = GetConfig();
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiBase}/folders/{folderId}");
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
        var (token, rootFolderId) = GetConfig();
        try
        {
            var folderId = await FindFolderPathAsync(token, remotePath, rootFolderId, ct);
            if (folderId is null)
                return [];

            using var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiBase}/folders/{folderId}/items?fields=id,name,size,modified_at,type");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            var entries = doc.RootElement.GetProperty("entries");

            return entries.EnumerateArray()
                .Where(e => e.GetProperty("type").GetString() == "file")
                .Select(e => new StorageItem
                {
                    Name = e.GetProperty("name").GetString() ?? string.Empty,
                    Path = $"box:{e.GetProperty("id").GetString()}",
                    SizeBytes = e.TryGetProperty("size", out var sz) ? sz.GetInt64() : 0,
                    LastModified = e.TryGetProperty("modified_at", out var dt)
                        && DateTime.TryParse(dt.GetString(), out var parsed) ? parsed : DateTime.MinValue,
                    IsDirectory = false
                }).ToList();
        }
        catch
        {
            return [];
        }
    }

    private async Task<string> EnsureFolderPathAsync(string token, string path, string parentId, CancellationToken ct)
    {
        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var current = parentId;

        foreach (var part in parts)
        {
            current = await EnsureFolderAsync(token, part, current, ct);
        }

        return current;
    }

    private async Task<string?> FindFolderPathAsync(string token, string path, string parentId, CancellationToken ct)
    {
        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var current = parentId;

        foreach (var part in parts)
        {
            var found = await FindFolderAsync(token, part, current, ct);
            if (found is null)
                return null;
            current = found;
        }

        return current;
    }

    private async Task<string?> FindFolderAsync(string token, string name, string parentId, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get,
            $"{ApiBase}/folders/{parentId}/items?fields=id,name,type");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _http.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);
        var entries = doc.RootElement.GetProperty("entries");

        foreach (var e in entries.EnumerateArray())
        {
            if (e.GetProperty("type").GetString() == "folder" &&
                e.GetProperty("name").GetString() == name)
            {
                return e.GetProperty("id").GetString();
            }
        }

        return null;
    }

    private async Task<string> EnsureFolderAsync(string token, string name, string parentId, CancellationToken ct)
    {
        var existing = await FindFolderAsync(token, name, parentId, ct);
        if (existing is not null)
            return existing;

        var body = JsonSerializer.Serialize(new
        {
            name,
            parent = new { id = parentId }
        });

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{ApiBase}/folders")
        {
            Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("id").GetString()!;
    }
}
