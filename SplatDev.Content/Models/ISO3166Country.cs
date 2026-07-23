namespace SplatDev.Content.Models
{
    /// <summary>
    /// Representation of an ISO3166-1 Country
    /// </summary>
    public class ISO3166Country
    {
        public ISO3166Country(string name, string alpha2, string alpha3, int numericCode, string[] dialCodes = null)
        {
            Name = name;
            Alpha2 = alpha2;
            Alpha3 = alpha3;
            NumericCode = numericCode;
            DialCodes = dialCodes;
        }

        public string Name { get; private set; }

        public string Alpha2 { get; private set; }

        public string Alpha3 { get; private set; }

        public int NumericCode { get; private set; }

        public string[] DialCodes { get; private set; }
    }
}
