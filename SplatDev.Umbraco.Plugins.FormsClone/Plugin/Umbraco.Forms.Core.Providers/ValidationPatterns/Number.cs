
// Type: Umbraco.Forms.Core.Providers.ValidationPatterns.Number
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Providers.ValidationPatterns
{
  public class Number : IValidationPattern
  {
    public string Alias => "number";

    public string Name => string.Empty;

    public string LabelKey => "validation_validateAsNumber";

    public string Pattern => "^[0-9]*$";

    public bool ReadOnly => true;
  }
}
