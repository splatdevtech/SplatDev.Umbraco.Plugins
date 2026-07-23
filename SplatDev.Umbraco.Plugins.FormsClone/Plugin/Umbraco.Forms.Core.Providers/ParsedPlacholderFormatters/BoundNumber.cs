
// Type: Umbraco.Forms.Core.Providers.ParsedPlacholderFormatters.BoundNumber
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using System.Globalization;

using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Providers.ParsedPlacholderFormatters
{
    public class BoundNumber : IParsedPlaceholderFormatter
    {
        public virtual string FunctionName => "bound";

        public virtual string FormatValue(string value, string[] args)
        {
            int result1;
            int result2;
            int result3;
            if (args.Length != 2 || !int.TryParse(args[0], out result1) || !int.TryParse(args[1], out result2) || !int.TryParse(value, out result3) && !int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out result3))
                return value;
            if (result3 < result1)
                return result1.ToString();
            return result3 > result2 ? result2.ToString() : result3.ToString();
        }
    }
}
