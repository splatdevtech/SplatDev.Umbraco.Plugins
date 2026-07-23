using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core.Services;

namespace SplatDev.Umbraco.Membership.Extensions
{
    public class RegisterExtensions(IMemberService memberService, ILogger<RegisterExtensions> logger)
    {

        private readonly IMemberService _memberService = memberService;
        private readonly ILogger<RegisterExtensions> _logger = logger;

        public void AssignMemberGroup(string email, string group)
        {
            try
            {
                _memberService.AssignRole(email, group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not assign member to group");
            }
        }

        public void QuoteInABoxOptIn(string email)
        {
            var umbracoMember = _memberService.GetByEmail(email!);
            if (umbracoMember is not null)
            {
                umbracoMember.SetValue("quoteInABox", true);
                //umbracoMember.SetValue("quoteInABoxDeliveryMethods", "[\"Email\"]");
                _memberService.Save(umbracoMember);
            }
        }
    }
}
