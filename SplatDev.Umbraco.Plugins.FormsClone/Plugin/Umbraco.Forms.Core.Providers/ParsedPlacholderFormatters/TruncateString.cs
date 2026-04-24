
// Type: Umbraco.Forms.Core.Providers.ParsedPlacholderFormatters.TruncateString
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Providers.ParsedPlacholderFormatters
{
  public class TruncateString : IParsedPlaceholderFormatter
  {
    public virtual string FunctionName => "truncate";

    public virtual string FormatValue(string value, string[] args)
    {
      int result;
      return args.Length == 0 || !int.TryParse(args[0], out result) || value.Length <= result ? value : value.Substring(0, result) + "...";
    }
  }
}
