namespace SplatDev.Umbraco.Plugins.Backups.Providers;

using System.Net.Http.Headers;
using System.Text.Json;
using SplatDev.Umbraco.Plugins.Backups.Configuration;
using SplatDev.Umbraco.Plugins.Backups.Models;

public class GoogleDriveStorageProvider : ICloudStorageProvider
{
    private readonly BackupSettings _settings;
    private static readonly HttpClient _http = new();

    private const string FilesApi = "https://www.googleapis.com/drive/v3/files";
    private const string UploadApi = "https://www.googleapis.com/upload/drive/v3/files?uploadType=multipart";

    public string ProviderName => "GoogleDrive";
    public string ProviderIcon => "google-drive";
    public bool RequiresOAuth => true;
    public bool RequiresApiKey => false;

    public GoogleDriveStorageProvider(BackupSettings settings)
    {
        _settings = settings;
    }

    private (string AccessToken, string FolderName) GetConfig()
    {
        var cfg = _settings.CloudProviders
            .FirstOrDefault(c => c.ProviderType == ProviderName && c.Enabled);
        if (cfg is null)
            throw new InvalidOperationException("Google Drive provider not configured.");

        var token = cfg.Settings.GetValueOrDefault("AccessToken") ?? string.Empty;
        var folder = cfg.Settings.GetValueOrDefault("FolderName") ?? "Backups";
        return (token, folder);
    }

    public async Task UploadAsync(Stream data, string remotePath, string fileName, CancellationToken ct)
    {
        var (token, rootFolder) = GetConfig();
        var folderId = await EnsureFolderPathAsync(token, $"{rootFolder}/{remotePath}", ct);

        var metadata = JsonSerializer.Serialize(new
        {
            name = fileName,
            parents = new[] { folderId }
        });

        using var content = new MultipartContent("related");
        content.Add(new StringContent(metadata, System.Text.Encoding.UTF8, "application/json"));
        var streamContent = new StreamContent(data);
        streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
        content.Add(streamContent);

        using var request = new HttpRequestMessage(HttpMethod.Post, UploadApi) { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<Stream> DownloadAsync(string remotePath, CancellationToken ct)
    {
        var (token, _) = GetConfig();
        var fileId = remotePath.StartsWith("gdrive:", StringComparison.Ordinal)
            ? remotePath["gdrive:".Length..]
            : remotePath;

        using var request = new HttpRequestMessage(HttpMethod.Get, $"{FilesApi}/{fileId}?alt=media");
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
        var fileId = remotePath.StartsWith("gdrive:", StringComparison.Ordinal)
            ? remotePath["gdrive:".Length..]
            : remotePath;

        using var request = new HttpRequestMessage(HttpMethod.Delete, $"{FilesApi}/{fileId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> ValidateConnectionAsync(CancellationToken ct)
    {
        try
        {
            var (token, _) = GetConfig();
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{FilesApi}?pageSize=1");
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
            var folderId = await FindFolderIdAsync(token, $"{rootFolder}/{remotePath}", ct);
            if (folderId is null)
                return [];

            var query = Uri.EscapeDataString($"'{folderId}' in parents and trashed=false");
            using var request = new HttpRequestMessage(HttpMethod.Get,
                $"{FilesApi}?q={query}&fields=files(id,name,size,modifiedTime)");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            var files = doc.RootElement.GetProperty("files");

            return files.EnumerateArray().Select(f => new StorageItem
            {
                Name = f.GetProperty("name").GetString() ?? string.Empty,
                Path = $"gdrive:{f.GetProperty("id").GetString()}",
                SizeBytes = long.TryParse(f.GetProperty("size").GetString(), out var sz) ? sz : 0,
                LastModified = DateTime.TryParse(f.GetProperty("modifiedTime").GetString(), out var dt) ? dt : DateTime.MinValue,
                IsDirectory = false
            }).ToList();
        }
        catch
        {
            return [];
        }
    }

    private async Task<string> EnsureFolderPathAsync(string token, string folderPath, CancellationToken ct)
    {
        var parts = folderPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var parentId = "root";

        foreach (var part in parts)
        {
            parentId = await EnsureFolderAsync(token, part, parentId, ct);
        }

        return parentId;
    }

    private async Task<string> EnsureFolderAsync(string token, string name, string parentId, CancellationToken ct)
    {
        var existing = await FindFolderInParentAsync(token, name, parentId, ct);
        if (existing is not null)
            return existing;

        var body = JsonSerializer.Serialize(new
        {
            name,
            mimeType = "application/vnd.google-apps.folder",
            parents = new[] { parentId }
        });

        using var request = new HttpRequestMessage(HttpMethod.Post, FilesApi)
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

    private async Task<string?> FindFolderInParentAsync(string token, string name, string parentId, CancellationToken ct)
    {
        var query = Uri.EscapeDataString(
            $"name='{name}' and '{parentId}' in parents and mimeType='application/vnd.google-apps.folder' and trashed=false");
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{FilesApi}?q={query}&fields=files(id)");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _http.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);
        var files = doc.RootElement.GetProperty("files");
        return files.GetArrayLength() > 0
            ? files[0].GetProperty("id").GetString()
            : null;
    }

    private async Task<string?> FindFolderIdAsync(string token, string folderPath, CancellationToken ct)
    {
        var parts = folderPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var parentId = "root";

        foreach (var part in parts)
        {
            var found = await FindFolderInParentAsync(token, part, parentId, ct);
            if (found is null)
                return null;
            parentId = found;
        }

        return parentId;
    }
}
