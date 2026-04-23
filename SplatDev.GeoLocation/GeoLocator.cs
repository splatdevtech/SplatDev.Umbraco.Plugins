namespace SplatDev.GeoLocation
{
    using RestSharp;

    using SplatDev.GeoLocation.Models;

    using System.Threading.Tasks;
    public class GeoLocator
    {
        public async Task<GeoLocationResult> GetIpInfoGeoLocation(string token, string ipAddress)
        {
            var client = new RestClient(Constants.APINFO);
            var request = new RestRequest(ipAddress);
            request.AddQueryParameter("token", token);
            var result = await client.GetAsync<GeoLocationResult>(request);
            return result;
        }
    }
}
