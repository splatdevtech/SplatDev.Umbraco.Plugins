using SplatDev.Umbraco.Plugins.Backups.Engine;
using Xunit;

namespace SplatDev.Umbraco.Plugins.Backups.Tests;

public class BackupEngineEncryptionTests
{
    [Fact]
    public async Task EncryptDecrypt_RoundTrip_ProducesOriginalContent()
    {
        var key = "test-encryption-key-12345";
        var originalContent = "This is a test backup payload content"u8.ToArray();
        var tempDir = Path.GetTempPath();
        var sourcePath = Path.Combine(tempDir, $"test-{Guid.NewGuid():N}.tmp");
        var encryptedPath = Path.Combine(tempDir, $"test-{Guid.NewGuid():N}.enc");
        var decryptedPath = Path.Combine(tempDir, $"test-{Guid.NewGuid():N}.dec");

        try
        {
            await File.WriteAllBytesAsync(sourcePath, originalContent);

            await BackupEngineTestHelpers.EncryptFileAsync(sourcePath, encryptedPath, key, CancellationToken.None);
            await BackupEngineTestHelpers.DecryptFileAsync(encryptedPath, decryptedPath, key, CancellationToken.None);

            var decryptedContent = await File.ReadAllBytesAsync(decryptedPath);
            Assert.Equal(originalContent, decryptedContent);
        }
        finally
        {
            foreach (var f in new[] { sourcePath, encryptedPath, decryptedPath })
            {
                if (File.Exists(f))
                    File.Delete(f);
            }
        }
    }

    [Fact]
    public async Task Encrypt_WithDifferentKeys_ProducesDifferentCiphertext()
    {
        var content = "same content"u8.ToArray();
        var tempDir = Path.GetTempPath();
        var sourcePath = Path.Combine(tempDir, $"test-{Guid.NewGuid():N}.tmp");
        var enc1 = Path.Combine(tempDir, $"test-{Guid.NewGuid():N}.enc");
        var enc2 = Path.Combine(tempDir, $"test-{Guid.NewGuid():N}.enc");

        try
        {
            await File.WriteAllBytesAsync(sourcePath, content);

            await BackupEngineTestHelpers.EncryptFileAsync(sourcePath, enc1, "key-one", CancellationToken.None);
            await BackupEngineTestHelpers.EncryptFileAsync(sourcePath, enc2, "key-two", CancellationToken.None);

            var bytes1 = await File.ReadAllBytesAsync(enc1);
            var bytes2 = await File.ReadAllBytesAsync(enc2);

            // Encrypted outputs differ (different IVs + keys)
            Assert.False(bytes1.SequenceEqual(bytes2));
        }
        finally
        {
            foreach (var f in new[] { sourcePath, enc1, enc2 })
            {
                if (File.Exists(f))
                    File.Delete(f);
            }
        }
    }

    [Fact]
    public async Task Decrypt_WithWrongKey_ThrowsOrProducesGarbage()
    {
        var content = "sensitive backup data"u8.ToArray();
        var tempDir = Path.GetTempPath();
        var sourcePath = Path.Combine(tempDir, $"test-{Guid.NewGuid():N}.tmp");
        var encPath = Path.Combine(tempDir, $"test-{Guid.NewGuid():N}.enc");
        var decPath = Path.Combine(tempDir, $"test-{Guid.NewGuid():N}.dec");

        try
        {
            await File.WriteAllBytesAsync(sourcePath, content);
            await BackupEngineTestHelpers.EncryptFileAsync(sourcePath, encPath, "correct-key", CancellationToken.None);

            var exception = await Record.ExceptionAsync(() =>
                BackupEngineTestHelpers.DecryptFileAsync(encPath, decPath, "wrong-key", CancellationToken.None));

            if (exception is null && File.Exists(decPath))
            {
                var decrypted = await File.ReadAllBytesAsync(decPath);
                Assert.False(decrypted.SequenceEqual(content), "Decryption with wrong key should not produce original content");
            }
        }
        finally
        {
            foreach (var f in new[] { sourcePath, encPath, decPath })
            {
                if (File.Exists(f))
                    File.Delete(f);
            }
        }
    }
}
