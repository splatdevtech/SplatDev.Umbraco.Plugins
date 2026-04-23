namespace SplatDev.Messaging.Models
{
    public class CannedMessagePlaceholder
    {
        public CannedMessagePlaceholder(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
