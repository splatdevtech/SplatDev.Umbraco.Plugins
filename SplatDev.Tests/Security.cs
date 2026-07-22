namespace SplatDev.Tests
{
    using Xunit;

    using SplatDev.Security;

    // SPL-2950: the URL-shortening tests below call external SaaS APIs
    // (CheckPhish, Google Safe Browsing, IPQualityScore) with API keys
    // previously hardcoded in this file (see SPL-2805). CI cannot reach
    // these providers reliably and keys are stale/invalid, producing
    // BadRequest and turning CI red on every push. Gate them under the
    // ExternalNetwork trait and filter them out of the CI test run.
    [Trait("Category", "ExternalNetwork")]
    public class Security
    {
        [Fact]
        public void UrlShortening_CheckPhish()
        {
            // arrange

            // act
            var response = Tools.CheckPhish("<test-api-key>", "http://maliciouswebsitetest.com/", true).GetAwaiter().GetResult();

            // assert
            Assert.NotNull(response);
        }

        [Fact]
        public void UrlShortening_GoogleSafeBrowsing()
        {
            // arrange

            // act
            var response = Tools.GoogleSafeBrowing("<test-api-key>", new string[] { "http://maliciouswebsitetest.com/" }, "splatdev").GetAwaiter().GetResult();

            // assert
            Assert.NotNull(response);
        }

        [Fact]
        public void UrlShortening_IpQualityScore()
        {
            // arrange

            // act
            var response = Tools.IpQualityScore("<test-api-key>", "http://maliciouswebsitetest.com/").GetAwaiter().GetResult();

            // assert
            Assert.NotNull(response);
        }
    }

    public class SecurityUnit
    {
        [Fact]
        public void Tools_EncodeBearerToken()
        {
            var response = Tools.EncodeAuthHeader("test-user", "test-password");
            Assert.NotNull(response);
        }
    }
}
