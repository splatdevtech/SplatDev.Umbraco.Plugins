using Microsoft.Extensions.Options;

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Security;

namespace SplatDev.Umbraco.Plugins.OAuth.Providers
{
    public class XMemberExternalLoginProviderOptions(IMemberService memberService) : IConfigureNamedOptions<MemberExternalLoginProviderOptions>
    {
        public const string SchemeName = "X";
        private readonly IMemberService _memberService = memberService;

        public void Configure(string? name, MemberExternalLoginProviderOptions options)
        {
            if (name != Constants.Security.MemberExternalAuthenticationTypePrefix + SchemeName)
            {
                return;
            }

            Configure(options);
        }

        public void Configure(MemberExternalLoginProviderOptions options)
        {
            options.AutoLinkOptions = new MemberExternalSignInAutoLinkOptions(
                autoLinkExternalAccount: true,
                defaultIsApproved: true,
                defaultMemberTypeAlias: "GeneralMember",
                defaultCulture: null,
                defaultMemberGroups: ["General"]
            )
            {
                OnExternalLogin = (member, loginInfo) =>
                {
                    var umbracoMember = _memberService.GetByEmail(member.Email!);
                    if (umbracoMember is not null)
                    {

                        var lastLoginDate = umbracoMember.GetValue<DateTime>("_umb_lastLoginDate");

                        if (lastLoginDate == default)
                        {
                            // This is the first login?
                            umbracoMember.SetValue("quoteInABox", true);
                            //umbracoMember.SetValue("quoteInABoxDeliveryMethods", "[\"Email\"]");
                            _memberService.Save(umbracoMember);
                        }
                    }

                    return true;
                }
            };
        }
    }
}