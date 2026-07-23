namespace SplatDev.Content.Models
{
    public class TimeZoneData
    {
        public string Abbreviation { get; set; }
        public string Name { get; set; }
        public string UtcOffsetDisplay { get; set; }
        public string EndOffset { get; set; }
        public TimeZoneData(string abbreviation, string name, string offset, string endOffset = "")
        {
            Abbreviation = abbreviation;
            Name = name;
            UtcOffsetDisplay = offset;
            if (!string.IsNullOrEmpty(endOffset)) EndOffset = endOffset;
        }
    }
}
