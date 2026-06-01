namespace SplatDev.Tests
{
    using Xunit;

    using SplatDev.GeoLocation;

    public class GeoLocation
    {
        [Fact(Skip = "Integration test — requires IpInfo API token")]
        public void GeoLocation_IpInfo()
        {
            // arrange

            // act
            var response = new GeoLocator().GetIpInfoGeoLocation("301524ff62217a", "152.254.243.1").GetAwaiter().GetResult();

            // assert
            Assert.NotNull(response);
        }
    }
}
