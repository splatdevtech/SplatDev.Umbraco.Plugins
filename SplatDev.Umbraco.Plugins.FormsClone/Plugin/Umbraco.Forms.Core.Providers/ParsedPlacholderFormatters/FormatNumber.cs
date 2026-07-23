
// Type: Umbraco.Forms.Core.Providers.ParsedPlacholderFormatters.FormatNumber
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using System;
using System.Globalization;
using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Providers.ParsedPlacholderFormatters
{
  public class FormatNumber : IParsedPlaceholderFormatter
  {
    public virtual string FunctionName => "number";

    public virtual string FormatValue(string value, string[] args)
    {
      if (args.Length == 0)
        return value;
      string format = args[0];
      int result1;
      if (int.TryParse(value, out result1) || int.TryParse(value, NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
        return result1.ToString(format);
      Decimal result2;
      return Decimal.TryParse(value, out result2) || Decimal.TryParse(value, NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture, out result2) ? result2.ToString(format) : value;
    }
  }
}
