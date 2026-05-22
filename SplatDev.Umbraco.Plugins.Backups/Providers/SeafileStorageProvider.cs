namespace SplatDev.Umbraco.Plugins.Backups.Providers;

using System.Net.Http.Headers;
using System.Text.Json;
using SplatDev.Umbraco.Plugins.Backups.Configuration;
using SplatDev.Umbraco.Plugins.Backups.Models;

public class SeafileStorageProvider : ICloudStorageProvider
{
    private readonly BackupSettings _settings;
    private static readonly HttpClient _http = new();

    public string ProviderName => "Seafile";
    public string ProviderIcon => "seafile";
    public bool RequiresOAuth => false;
    public bool RequiresApiKey => true;

    public SeafileStorageProvider(BackupSettings settings)
    {
        _settings = settings;
    }

    private (string ServerUrl, string Token, string LibraryId, string FolderPath) GetConfig()
    {
        var cfg = _settings.CloudProviders
            .FirstOrDefault(c => c.ProviderType == ProviderName && c.Enabled);
        if (cfg is null)
            throw new InvalidOperationException("Seafile provider not configured.");

        var server = (cfg.Settings.GetValueOrDefault("ServerUrl") ?? string.Empty).TrimEnd('/');
        var token = cfg.Settings.GetValueOrDefault("Token") ?? string.Empty;
        var libraryId = cfg.Settings.GetValueOrDefault("LibraryId") ?? string.Empty;
        var folder = cfg.Settings.GetValueOrDefault("FolderPath") ?? "/Backups";
        if (!folder.StartsWith('/'))
            folder = "/" + folder;

        return (server, token, libraryId, folder);
    }

    public async Task UploadAsync(Stream data, string remotePath, string fileName, CancellationToken ct)
    {
        var (server, token, libraryId, rootFolder) = GetConfig();
        var targetDir = $"{rootFolder}/{remotePath}".Replace("//", "/");

        await EnsureDirectoryAsync(server, token, libraryId, targetDir, ct);

        var uploadUrl = await GetUploadLinkAsync(server, token, libraryId, targetDir, ct);

        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(data);
        streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
        content.Add(streamContent, "file", fileName);
        content.Add(new StringContent(targetDir), "parent_dir");
        content.Add(new StringContent("1"), "replace");

        using var request = new HttpRequestMessage(HttpMethod.Post, uploadUrl) { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue("Token", token);

        var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<Stream> DownloadAsync(string remotePath, CancellationToken ct)
    {
        var (server, token, libraryId, _) = GetConfig();
        var encodedPath = Uri.EscapeDataString(remotePath);

        using var linkRequest = new HttpRequestMessage(HttpMethod.Get,
            $"{server}/api2/repos/{libraryId}/file/?p={encodedPath}");
        linkRequest.Headers.Authorization = new AuthenticationHeaderValue("Token", token);

        var linkResponse = await _http.SendAsync(linkRequest, ct);
        linkResponse.EnsureSuccessStatusCode();

        var downloadUrl = (await linkResponse.Content.ReadAsStringAsync(ct)).Trim('"');

        using var fileRequest = new HttpRequestMessage(HttpMethod.Get, downloadUrl);
        fileRequest.Headers.Authorization = new AuthenticationHeaderValue("Token", token);

        var fileResponse = await _http.SendAsync(fileRequest, HttpCompletionOption.ResponseHeadersRead, ct);
        fileResponse.EnsureSuccessStatusCode();

        var ms = new MemoryStream();
        await fileResponse.Content.CopyToAsync(ms, ct);
        ms.Position = 0;
        return ms;
    }

    public async Task DeleteAsync(string remotePath, CancellationToken ct)
    {
        var (server, token, libraryId, _) = GetConfig();
        var encodedPath = Uri.EscapeDataString(remotePath);

        using var request = new HttpRequestMessage(HttpMethod.Delete,
            $"{server}/api2/repos/{libraryId}/file/?p={encodedPath}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Token", token);

        var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> ValidateConnectionAsync(CancellationToken ct)
    {
        try
        {
            var (server, token, libraryId, _) = GetConfig();
            using var request = new HttpRequestMessage(HttpMethod.Get,
                $"{server}/api2/repos/{libraryId}/");
            request.Headers.Authorization = new AuthenticationHeaderValue("Token", token);

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
        var (server, token, libraryId, rootFolder) = GetConfig();
        try
        {
            var dir = $"{rootFolder}/{remotePath}".Replace("//", "/");
            var encodedDir = Uri.EscapeDataString(dir);

            using var request = new HttpRequestMessage(HttpMethod.Get,
                $"{server}/api2/repos/{libraryId}/dir/?p={encodedDir}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Token", token);

            var response = await _http.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
                return [];

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement.EnumerateArray()
                .Where(e => e.GetProperty("type").GetString() == "file")
                .Select(e => new StorageItem
                {
                    Name = e.GetProperty("name").GetString() ?? string.Empty,
                    Path = $"{dir}/{e.GetProperty("name").GetString()}",
                    SizeBytes = e.TryGetProperty("size", out var sz) ? sz.GetInt64() : 0,
                    LastModified = e.TryGetProperty("mtime", out var mt)
                        ? DateTimeOffset.FromUnixTimeSeconds(mt.GetInt64()).UtcDateTime
                        : DateTime.MinValue,
                    IsDirectory = false
                }).ToList();
        }
        catch
        {
            return [];
        }
    }

    private async Task<string> GetUploadLinkAsync(string server, string token, string libraryId, string dir, CancellationToken ct)
    {
        var encodedDir = Uri.EscapeDataString(dir);
        using var request = new HttpRequestMessage(HttpMethod.Get,
            $"{server}/api2/repos/{libraryId}/upload-link/?p={encodedDir}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Token", token);

        var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadAsStringAsync(ct)).Trim('"');
    }

    private async Task EnsureDirectoryAsync(string server, string token, string libraryId, string dir, CancellationToken ct)
    {
        var encodedDir = Uri.EscapeDataString(dir);
        using var checkRequest = new HttpRequestMessage(HttpMethod.Get,
            $"{server}/api2/repos/{libraryId}/dir/?p={encodedDir}");
        checkRequest.Headers.Authorization = new AuthenticationHeaderValue("Token", token);

        var checkResponse = await _http.SendAsync(checkRequest, ct);
        if (checkResponse.IsSuccessStatusCode)
            return;

        using var createRequest = new HttpRequestMessage(HttpMethod.Post,
            $"{server}/api2/repos/{libraryId}/dir/?p={encodedDir}")
        {
            Content = new StringContent("operation=mkdir", System.Text.Encoding.UTF8, "application/x-www-form-urlencoded")
        };
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Token", token);

        await _http.SendAsync(createRequest, ct);
    }
}
