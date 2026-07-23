namespace SplatDev.Messaging.Models
{
    using System.Collections.Generic;
    public static class DefaultPlaceholders
    {
        public const string NAME = "##NAME##";
        public const string EMAIL = "##EMAIL##";
        public const string ID = "##ID##";
        public const string PAYLINK = "##PAYLINK##";
        public const string DATE = "##DATE##";
        public const string TIME = "##EVENTNAME##";
        public const string ROOTURL = "##IPADDRESS##";
        public const string MEMBER = "##MEMBER##";
        public const string VIEWURL = "##VIEWURL##";
        public const string REFCODE = "##REFCODE##";
        public const string PAGEURL = "##PAGEURL##";
        public const string GUID = "##GUID##";

        public static List<CannedMessagePlaceholder> Placeholders { get; set; }

        public static void AddPlaceholder(string key, string value)
        {
            Placeholders.Add(new CannedMessagePlaceholder(key, value));
        }

        public static CannedMessagePlaceholder Placeholder(string key)
        {
            if (Placeholders == null) Placeholders = new List<CannedMessagePlaceholder>();
            return Placeholders.Find(x => x.Key == key);
        }

        public static string Value(string key)
        {
            return Placeholder(key).Value;
        }

        public static List<CannedMessagePlaceholder> PopulateDefault(string rootUrl = "/")
        {
            Placeholders.AddRange(new List<CannedMessagePlaceholder>
            {
                new CannedMessagePlaceholder(NAME, NAME),
                new CannedMessagePlaceholder(EMAIL, EMAIL),
                new CannedMessagePlaceholder(ID, ID),
                new CannedMessagePlaceholder(PAYLINK, PAYLINK),
                new CannedMessagePlaceholder(DATE, DATE),
                new CannedMessagePlaceholder(TIME, TIME),
                new CannedMessagePlaceholder(ROOTURL, rootUrl),
                new CannedMessagePlaceholder(MEMBER, MEMBER),
                new CannedMessagePlaceholder(REFCODE, REFCODE),
                new CannedMessagePlaceholder(PAGEURL, PAGEURL),
                new CannedMessagePlaceholder(GUID, GUID),
                new CannedMessagePlaceholder(VIEWURL, VIEWURL)
            });
            return Placeholders;
        }
    }
}