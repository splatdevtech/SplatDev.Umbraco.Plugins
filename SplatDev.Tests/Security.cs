namespace SplatDev.Tests
{
    using Xunit;

    using SplatDev.Security;

    public class Security
    {
        [Fact]
        [Trait("Category", "Integration")]
        public void UrlShortening_CheckPhish()
        {
            var response = Tools.CheckPhish("ux904vysg7jhbakonldt53cmzhyrgdxc19c0xvnbbrffr83qshrdrozbu3lqqtb0", "http://maliciouswebsitetest.com/", true).GetAwaiter().GetResult();
            Assert.NotNull(response);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void UrlShortening_GoogleSafeBrowsing()
        {
            var response = Tools.GoogleSafeBrowing("AIzaSyCHCEGziKvWkas-zw7DFSQ4W_66mlDQKN8", new string[] { "http://maliciouswebsitetest.com/" }, "splatdev").GetAwaiter().GetResult();
            Assert.NotNull(response);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void UrlShortening_IpQualityScore()
        {
            var response = Tools.IpQualityScore("kNc64pBCcMNpQcW2cR85bQsKhzbnnxVw", "http://maliciouswebsitetest.com/").GetAwaiter().GetResult();
            Assert.NotNull(response);
        }

        [Fact]
        public void Tools_EncodeBearerToken()
        {
            var response = Tools.EncodeAuthHeader("ccasalicchio", "tWA5@hr3Gug6Ahq");
            Assert.NotNull(response);
        }
    }
}
