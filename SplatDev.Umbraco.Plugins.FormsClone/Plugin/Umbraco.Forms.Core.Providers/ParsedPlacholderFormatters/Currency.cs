
// Type: Umbraco.Forms.Core.Providers.ParsedPlacholderFormatters.Currency
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68


#nullable enable
namespace Umbraco.Forms.Core.Providers.ParsedPlacholderFormatters
{
    public class Currency : FormatNumber
    {
        public override string FunctionName => "currency";

        public override string FormatValue(string value, string[] args) => FormatValue(value, new string[1]
        {
      "C"
        });
    }
}
