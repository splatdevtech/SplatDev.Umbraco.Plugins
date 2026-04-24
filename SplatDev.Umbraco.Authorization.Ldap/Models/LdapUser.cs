using Umbraco.Cms.Core.Models.Membership;

namespace SplatDev.Umbraco.Authorization.Ldap.Models
{
    public class LdapUser(bool isAllowed)
    {
        public string Username { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string[]? Groups { get; set; } = [];
        public bool IsAllowed { get; private set; } = isAllowed;
        public bool IsAllowedBackOffice { get; set; } = false;
        public IUser? UmbracoUser { get; set; }
    }
}
