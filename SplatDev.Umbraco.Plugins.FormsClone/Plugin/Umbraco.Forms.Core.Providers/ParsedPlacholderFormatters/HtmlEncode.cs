
// Type: Umbraco.Forms.Core.Providers.ParsedPlacholderFormatters.HtmlEncode
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using System.Net;
using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Providers.ParsedPlacholderFormatters
{
  public class HtmlEncode : IParsedPlaceholderFormatter
  {
    public virtual string FunctionName => "html";

    public virtual string FormatValue(string value, string[] args) => WebUtility.HtmlEncode(value);
  }
}
