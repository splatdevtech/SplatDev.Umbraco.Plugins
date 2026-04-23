namespace SplatDev.Tests
{
    using Xunit;

    using SplatDev.Security;

    public class Security
    {
        [Fact]
        public void UrlShortening_CheckPhish()
        {
            // arrange

            // act
            var response = Tools.CheckPhish("ux904vysg7jhbakonldt53cmzhyrgdxc19c0xvnbbrffr83qshrdrozbu3lqqtb0", "http://maliciouswebsitetest.com/", true).GetAwaiter().GetResult();

            // assert
            Assert.NotNull(response);
        }

        [Fact]
        public void UrlShortening_GoogleSafeBrowsing()
        {
            // arrange

            // act
            var response = Tools.GoogleSafeBrowing("AIzaSyCHCEGziKvWkas-zw7DFSQ4W_66mlDQKN8", new string[] { "http://maliciouswebsitetest.com/" }, "splatdev").GetAwaiter().GetResult();

            // assert
            Assert.NotNull(response);
        }

        [Fact]
        public void UrlShortening_IpQualityScore()
        {
            // arrange

            // act
            var response = Tools.IpQualityScore("kNc64pBCcMNpQcW2cR85bQsKhzbnnxVw", "http://maliciouswebsitetest.com/").GetAwaiter().GetResult();

            // assert
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
