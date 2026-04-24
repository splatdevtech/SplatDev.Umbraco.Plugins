
// Type: Umbraco.Forms.Core.Providers.ValidationPatterns.Url
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Providers.ValidationPatterns
{
  public class Url : IValidationPattern
  {
    public string Alias => "url";

    public string Name => string.Empty;

    public string LabelKey => "validation_validateAsUrl";

    public string Pattern => "https?\\:\\/\\/[a-zA-Z0-9\\-\\.]+\\.[a-zA-Z]{2,}";

    public bool ReadOnly => true;
  }
}
