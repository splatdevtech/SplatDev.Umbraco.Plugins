
// Type: Umbraco.Forms.Core.Configuration.RecaptchaDomainExtensions
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Forms.Core.Extensions;


#nullable enable
namespace Umbraco.Forms.Core.Configuration
{
  public static class RecaptchaDomainExtensions
  {
    public static string GetDomainName(this RecaptchaDomain domain) => domain.GetDescription<RecaptchaDomain>();
  }
}
