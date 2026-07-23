using FormBuilder.Core.Configuration;

namespace FormBuilder.Core.Extensions
{
    public static class RecaptchaDomainExtensions
    {
        public static string GetDomainName(this RecaptchaDomain domain) => domain.GetDescription();
    }
}