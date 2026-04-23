using System;

namespace SplatDev.UrlShortening.Models
{
    public class CheckPhishResponse
    {
#pragma warning disable IDE1006 // Naming Styles
        public string jobID { get; set; }
        public Guid job_id { get; set; }
        public long timestamp { get; set; }
        public string status { get; set; }
        public string url { get; set; }
        public string url_Sha256 { get; set; }
        public string disposition { get; set; }
        public string screenshot_path { get; set; }
        public bool resolved { get; set; }
        public bool error { get; set; }
        public string brand { get; set; }
        public string insights { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
