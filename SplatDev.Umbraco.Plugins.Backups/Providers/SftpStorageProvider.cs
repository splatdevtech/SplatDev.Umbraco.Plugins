namespace SplatDev.Umbraco.Plugins.Backups.Providers;

using Renci.SshNet;
using SplatDev.Umbraco.Plugins.Backups.Configuration;
using SplatDev.Umbraco.Plugins.Backups.Models;

public class SftpStorageProvider : ICloudStorageProvider
{
    private readonly BackupSettings _settings;

    public string ProviderName => "Sftp";
    public string ProviderIcon => "sftp";
    public bool RequiresOAuth => false;
    public bool RequiresApiKey => true;

    public SftpStorageProvider(BackupSettings settings)
    {
        _settings = settings;
    }

    private (string Host, int Port, string Username, string Password, string PrivateKeyPath, string RemotePath) GetConfig()
    {
        var cfg = _settings.CloudProviders
            .FirstOrDefault(c => c.ProviderType == ProviderName && c.Enabled);
        if (cfg is null)
            throw new InvalidOperationException("SFTP provider not configured.");

        var host = cfg.Settings.GetValueOrDefault("Host") ?? string.Empty;
        var portStr = cfg.Settings.GetValueOrDefault("Port") ?? "22";
        var port = int.TryParse(portStr, out var p) ? p : 22;
        var username = cfg.Settings.GetValueOrDefault("Username") ?? string.Empty;
        var password = cfg.Settings.GetValueOrDefault("Password") ?? string.Empty;
        var keyPath = cfg.Settings.GetValueOrDefault("PrivateKeyPath") ?? string.Empty;
        var remotePath = (cfg.Settings.GetValueOrDefault("RemotePath") ?? "/backups").TrimEnd('/');

        return (host, port, username, password, keyPath, remotePath);
    }

    private SftpClient CreateClient()
    {
        var (host, port, username, password, keyPath, _) = GetConfig();

        if (!string.IsNullOrEmpty(keyPath) && File.Exists(keyPath))
        {
            var privateKey = string.IsNullOrEmpty(password)
                ? new PrivateKeyFile(keyPath)
                : new PrivateKeyFile(keyPath, password);

            return new SftpClient(new ConnectionInfo(host, port, username,
                new PrivateKeyAuthenticationMethod(username, privateKey)));
        }

        return new SftpClient(new ConnectionInfo(host, port, username,
            new PasswordAuthenticationMethod(username, password)));
    }

    public async Task UploadAsync(Stream data, string remotePath, string fileName, CancellationToken ct)
    {
        var (_, _, _, _, _, baseRemotePath) = GetConfig();
        var targetDir = $"{baseRemotePath}/{remotePath}".Replace("//", "/");
        var targetPath = $"{targetDir}/{fileName}";

        using var client = CreateClient();
        client.Connect();

        EnsureDirectory(client, targetDir);
        await client.UploadFileAsync(data, targetPath, cancellationToken: ct);
        client.Disconnect();
    }

    public async Task<Stream> DownloadAsync(string remotePath, CancellationToken ct)
    {
        using var client = CreateClient();
        client.Connect();

        var ms = new MemoryStream();
        await client.DownloadFileAsync(remotePath, ms, cancellationToken: ct);
        ms.Position = 0;

        client.Disconnect();
        return ms;
    }

    public Task DeleteAsync(string remotePath, CancellationToken ct)
    {
        using var client = CreateClient();
        client.Connect();
        client.DeleteFile(remotePath);
        client.Disconnect();
        return Task.CompletedTask;
    }

    public Task<bool> ValidateConnectionAsync(CancellationToken ct)
    {
        try
        {
            using var client = CreateClient();
            client.Connect();
            var connected = client.IsConnected;
            client.Disconnect();
            return Task.FromResult(connected);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public async Task<IEnumerable<StorageItem>> ListAsync(string remotePath, CancellationToken ct)
    {
        var (_, _, _, _, _, baseRemotePath) = GetConfig();
        try
        {
            using var client = CreateClient();
            client.Connect();

            var dir = $"{baseRemotePath}/{remotePath}".Replace("//", "/");
            if (!client.Exists(dir))
            {
                client.Disconnect();
                return [];
            }

            var items = new List<StorageItem>();
            await foreach (var file in client.ListDirectoryAsync(dir, ct))
            {
                if (file.Name is "." or "..")
                    continue;

                items.Add(new StorageItem
                {
                    Name = file.Name,
                    Path = file.FullName,
                    SizeBytes = file.Length,
                    LastModified = file.LastWriteTimeUtc,
                    IsDirectory = file.IsDirectory
                });
            }

            client.Disconnect();
            return items;
        }
        catch
        {
            return [];
        }
    }

    private static void EnsureDirectory(SftpClient client, string path)
    {
        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var current = "/";

        foreach (var part in parts)
        {
            current = current.TrimEnd('/') + "/" + part;
            if (!client.Exists(current))
                client.CreateDirectory(current);
        }
    }
}
