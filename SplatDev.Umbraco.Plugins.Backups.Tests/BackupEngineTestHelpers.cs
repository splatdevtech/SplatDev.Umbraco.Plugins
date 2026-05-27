using System.Security.Cryptography;

namespace SplatDev.Umbraco.Plugins.Backups.Tests;

internal static class BackupEngineTestHelpers
{
    internal static async Task EncryptFileAsync(string sourcePath, string destPath, string key, CancellationToken ct)
    {
        var keyBytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(key));

        await using var sourceStream = File.OpenRead(sourcePath);
        await using var destStream = File.Create(destPath);

        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.GenerateIV();
        await destStream.WriteAsync(aes.IV, ct);

        await using var cryptoStream = new CryptoStream(destStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
        await sourceStream.CopyToAsync(cryptoStream, ct);
        await cryptoStream.FlushFinalBlockAsync(ct);
    }

    internal static async Task DecryptFileAsync(string sourcePath, string destPath, string key, CancellationToken ct)
    {
        var keyBytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(key));

        await using var sourceStream = File.OpenRead(sourcePath);
        var iv = new byte[16];
        await sourceStream.ReadExactlyAsync(iv, ct);

        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.IV = iv;

        await using var cryptoStream = new CryptoStream(sourceStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
        await using var destStream = File.Create(destPath);
        await cryptoStream.CopyToAsync(destStream, ct);
    }
}
