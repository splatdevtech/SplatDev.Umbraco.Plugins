namespace SplatDev.UrlShortening.Models
{
    public class IpQualityScoreResponse
    {
        /*
         * {
   "message":"Success.",
   "success":true,
   "unsafe":false,
   "domain":"splatdev.com",
   "ip_address":"104.21.81.29",
   "server":" cloudflare\r\n",
   "content_type":"text\/html; charset=utf-8",
   "status_code":200,
   "page_size":5791,
   "domain_rank":0,
   "dns_valid":true,
   "parking":false,
   "spamming":false,
   "malware":false,
   "phishing":false,
   "suspicious":false,
   "adult":false,
   "risk_score":46,
   "category":"Business",
   "domain_age":{
      "human":"2 years ago",
      "timestamp":1556629929,
      "iso":"2019-04-30T09:12:09-04:00"
   },
   "request_id":"4DtCFq4HAXQEdCZ"
}
         * 
         */

        public string Message { get; set; }
        public bool Success { get; set; }
        public bool Unsafe { get; set; }
        public string Domain { get; set; }
        public string Ip_address { get; set; }
        public string Server { get; set; }
        public string Content_type { get; set; }
        public int Status_code { get; set; }
        public int Page_size { get; set; }
        public bool Dns_valid { get; set; }
        public bool Parking { get; set; }
        public bool Spamming { get; set; }
        public bool Malware { get; set; }
        public bool Phishing { get; set; }
        public bool Suspicious { get; set; }
        public bool Adult { get; set; }
        public int Risk_score { get; set; }
        public string Category { get; set; }
        public DomainAge Domain_age { get; set; }
        public string Request_id { get; set; }

    }

    public struct DomainAge
    {
        public string Human { get; set; }
        public long Timestamp { get; set; }
        public string Iso { get; set; }
    }
}
