namespace SplatDev.Payments.MercadoPago.Requests
{
    using global::MercadoPago.Config;

    using RestSharp;
    using RestSharp.Serializers.NewtonsoftJson;

    using SplatDev.Payments.MercadoPago.Models;

    using System.Threading.Tasks;

    public class UserRequest
    {
        private readonly string ACCESS_TOKEN;
        private readonly RestClient client;

        public UserRequest(string accessToken)
        {
            ACCESS_TOKEN = accessToken;
            MercadoPagoConfig.AccessToken = ACCESS_TOKEN;
            client = new RestClient(Constants.APIv1);
            client.UseNewtonsoftJson(Constants.API_JSON_SETTINGS);
            client.AddDefaultHeader("Authorization", $"Bearer {ACCESS_TOKEN}");
        }

        public async Task<User> CreateTestUser(string siteId = "MLB")
        {
            var request = new RestRequest("/users/test_user")
            {
#pragma warning disable CS0618 // Type or member is obsolete
                Body = new RequestBody("application/json", null, new { site_id = siteId })
#pragma warning restore CS0618 // Type or member is obsolete
            };
            return await client.PostAsync<User>(request);
        }
    }
}
