namespace SplatDev.GeoLocation.Models
{
    public class GeoLocationResult
    {
        /*
        "ip": "152.254.243.1",
        "hostname": "152-254-243-1.user.vivozap.com.br",
        "city": "Campinas",
        "region": "São Paulo",
        "country": "BR",
        "loc": "-22.9056,-47.0608",
        "org": "AS27699 TELEFÔNICA BRASIL S.A",
        "postal": "13000-000",
        "timezone": "America/Sao_Paulo"
        */
        public string Ip { get; set; }
        public string Hostname { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string Loc { get; set; }
        public string Org { get; set; }
        public string Postal { get; set; }
        public string Timezone { get; set; }
    }
}
