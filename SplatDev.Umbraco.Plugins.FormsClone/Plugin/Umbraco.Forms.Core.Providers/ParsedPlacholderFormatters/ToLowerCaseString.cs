
// Type: Umbraco.Forms.Core.Providers.ParsedPlacholderFormatters.ToLowerCaseString
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Providers.ParsedPlacholderFormatters
{
  public class ToLowerCaseString : IParsedPlaceholderFormatter
  {
    public virtual string FunctionName => "lower";

    public virtual string FormatValue(string value, string[] args) => value.ToLower();
  }
}
