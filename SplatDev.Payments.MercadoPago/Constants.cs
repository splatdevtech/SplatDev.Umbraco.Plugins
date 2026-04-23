namespace SplatDev.Payments.MercadoPago
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class Constants
    {
        public const string APIv1 = "https://api.mercadopago.com/v1/";
        public const string API = "https://api.mercadopago.com/";
        public static JsonSerializerSettings API_JSON_SETTINGS = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() },
            DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ",
            DefaultValueHandling = DefaultValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None,
            ConstructorHandling = ConstructorHandling.Default
        };
    }
}
