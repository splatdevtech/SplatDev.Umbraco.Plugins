
// Type: Umbraco.Forms.Core.Providers.ValidationPatterns.Email
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Providers.ValidationPatterns
{
  public class Email : IValidationPattern
  {
    public string Alias => "email";

    public string Name => string.Empty;

    public string LabelKey => "validation_validateAsEmail";

    public string Pattern => "^[a-zA-Z0-9_\\.\\+-]+@[a-zA-Z0-9-]+\\.[a-zA-Z0-9-\\.]+$";

    public bool ReadOnly => true;
  }
}
