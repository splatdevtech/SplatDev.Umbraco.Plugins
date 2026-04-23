
// Type: Umbraco.Forms.Core.Providers.EmailTemplates.DefaultEmailTemplate
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Providers.EmailTemplates
{
    public class DefaultEmailTemplate : IEmailTemplate
    {
        public virtual string FileName => "Example-Template.cshtml";
    }
}
