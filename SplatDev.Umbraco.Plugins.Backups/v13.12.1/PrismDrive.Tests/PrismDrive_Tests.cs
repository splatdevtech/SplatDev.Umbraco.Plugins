using Microsoft.AspNetCore.Http.Internal;

using SplatDev.Plugins.BackupVault.Services;

using System.Text;

namespace PrismDrive.Tests
{
    [TestClass]
    public class PrismDrive_Tests
    {
        private const string username = "carlos.casalicchio@gmail.com";
        private const string password = "d9rySDxxruXPgcE6XL2CdJsQXHn6s2dY";
        private const string tokenName = "uPlugins.Backups";

        [TestMethod]
        public void PrismDrive_Tests__Login()
        {
            var service = new PrismDriveService();
            var response = PrismDriveService.LoginAsync(username, password, tokenName).Result;
            Assert.IsNotNull(response.AccessToken);
        }

        [TestMethod]
        public async Task PrismDrive_Tests__UploadAsync()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("This is a test"));
            var file = new FormFile(stream, 0, stream.Length, "Test", "test.txt");

            var service = new PrismDriveService();
            var login = await PrismDriveService.LoginAsync(username, password, tokenName);
            var response = await PrismDriveService.UploadAsync(file, login.AccessToken, relativePath: "/test/test.txt");
            Assert.IsNotNull(response);
        }
    }
}