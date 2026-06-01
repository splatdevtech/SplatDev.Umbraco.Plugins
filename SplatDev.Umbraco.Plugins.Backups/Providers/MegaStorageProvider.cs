namespace SplatDev.Umbraco.Plugins.Backups.Providers;

using CG.Web.MegaApiClient;
using SplatDev.Umbraco.Plugins.Backups.Configuration;
using SplatDev.Umbraco.Plugins.Backups.Models;

public class MegaStorageProvider : ICloudStorageProvider
{
    private readonly BackupSettings _settings;

    public string ProviderName => "Mega";
    public string ProviderIcon => "mega";
    public bool RequiresOAuth => false;
    public bool RequiresApiKey => false;

    public MegaStorageProvider(BackupSettings settings)
    {
        _settings = settings;
    }

    private (string Email, string Password, string FolderPath) GetConfig()
    {
        var cfg = _settings.CloudProviders
            .FirstOrDefault(c => c.ProviderType == ProviderName && c.Enabled);
        if (cfg is null)
            throw new InvalidOperationException("Mega provider not configured.");

        var email = cfg.Settings.GetValueOrDefault("Email") ?? string.Empty;
        var password = cfg.Settings.GetValueOrDefault("Password") ?? string.Empty;
        var folder = cfg.Settings.GetValueOrDefault("FolderPath") ?? "Backups";
        return (email, password, folder);
    }

    public async Task UploadAsync(Stream data, string remotePath, string fileName, CancellationToken ct)
    {
        var (email, password, rootFolder) = GetConfig();
        var client = new MegaApiClient();

        await client.LoginAsync(email, password);
        try
        {
            var nodes = await client.GetNodesAsync();
            var root = nodes.Single(n => n.Type == NodeType.Root);
            var folder = await EnsureFolderPathAsync(client, nodes, $"{rootFolder}/{remotePath}", root);
            await client.UploadAsync(data, fileName, folder, null, null, ct);
        }
        finally
        {
            await client.LogoutAsync();
        }
    }

    public async Task<Stream> DownloadAsync(string remotePath, CancellationToken ct)
    {
        var (email, password, _) = GetConfig();
        var client = new MegaApiClient();

        await client.LoginAsync(email, password);
        try
        {
            var nodes = await client.GetNodesAsync();
            var nodeId = remotePath.StartsWith("mega:", StringComparison.Ordinal)
                ? remotePath["mega:".Length..]
                : remotePath;

            var node = nodes.FirstOrDefault(n => n.Id == nodeId)
                ?? throw new FileNotFoundException($"Node not found: {nodeId}");

            var ms = new MemoryStream();
            using var downloadStream = await client.DownloadAsync(node, null, ct);
            await downloadStream.CopyToAsync(ms, ct);
            ms.Position = 0;
            return ms;
        }
        finally
        {
            await client.LogoutAsync();
        }
    }

    public async Task DeleteAsync(string remotePath, CancellationToken ct)
    {
        var (email, password, _) = GetConfig();
        var client = new MegaApiClient();

        await client.LoginAsync(email, password);
        try
        {
            var nodes = await client.GetNodesAsync();
            var nodeId = remotePath.StartsWith("mega:", StringComparison.Ordinal)
                ? remotePath["mega:".Length..]
                : remotePath;

            var node = nodes.FirstOrDefault(n => n.Id == nodeId);
            if (node is not null)
                await client.DeleteAsync(node, moveToTrash: false);
        }
        finally
        {
            await client.LogoutAsync();
        }
    }

    public async Task<bool> ValidateConnectionAsync(CancellationToken ct)
    {
        try
        {
            var (email, password, _) = GetConfig();
            var client = new MegaApiClient();
            await client.LoginAsync(email, password);
            try
            {
                await client.LogoutAsync();
            }
            catch { /* swallow logout errors on validation */ }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<StorageItem>> ListAsync(string remotePath, CancellationToken ct)
    {
        var (email, password, rootFolder) = GetConfig();
        var client = new MegaApiClient();

        await client.LoginAsync(email, password);
        try
        {
            var nodes = await client.GetNodesAsync();
            var root = nodes.Single(n => n.Type == NodeType.Root);
            var folder = FindFolderPath(nodes, $"{rootFolder}/{remotePath}", root);

            if (folder is null)
                return [];

            return nodes
                .Where(n => n.ParentId == folder.Id && n.Type == NodeType.File)
                .Select(n => new StorageItem
                {
                    Name = n.Name ?? string.Empty,
                    Path = $"mega:{n.Id}",
                    SizeBytes = n.Size,
                    LastModified = n.ModificationDate ?? n.CreationDate ?? DateTime.MinValue,
                    IsDirectory = false
                }).ToList();
        }
        finally
        {
            await client.LogoutAsync();
        }
    }

    private static async Task<INode> EnsureFolderPathAsync(
        IMegaApiClient client,
        IEnumerable<INode> nodes,
        string path,
        INode parent)
    {
        var nodeList = nodes.ToList();
        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var current = parent;

        foreach (var part in parts)
        {
            var existing = nodeList.FirstOrDefault(n =>
                n.Type == NodeType.Directory &&
                n.ParentId == current.Id &&
                n.Name == part);

            if (existing is not null)
            {
                current = existing;
            }
            else
            {
                current = await client.CreateFolderAsync(part, current);
                nodeList = (await client.GetNodesAsync()).ToList();
            }
        }

        return current;
    }

    private static INode? FindFolderPath(IEnumerable<INode> nodes, string path, INode root)
    {
        var nodeList = nodes.ToList();
        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var current = root;

        foreach (var part in parts)
        {
            var found = nodeList.FirstOrDefault(n =>
                n.Type == NodeType.Directory &&
                n.ParentId == current.Id &&
                n.Name == part);

            if (found is null)
                return null;

            current = found;
        }

        return current;
    }
}
