
// Type: Umbraco.Forms.Core.Providers.ParsedPlacholderFormatters.FormatDate
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using System.Globalization;

using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Providers.ParsedPlacholderFormatters
{
    public class FormatDate : IParsedPlaceholderFormatter
    {
        public virtual string FunctionName => "date";

        public virtual string FormatValue(string value, string[] args)
        {
            if (args.Length == 0)
                return value;
            string format = args[0];
            DateTime result;
            return DateTime.TryParse(value, out result) || DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out result) ? result.ToString(format) : value;
        }
    }
}
