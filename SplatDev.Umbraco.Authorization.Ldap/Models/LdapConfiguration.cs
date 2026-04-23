#if !NET10_0_OR_GREATER
using Umbraco.Cms.Core.Models;
#endif

namespace SplatDev.Umbraco.Authorization.Ldap.Models
{
    public class LdapConfiguration
    {
        public const string LdapSectionName = nameof(LdapConfiguration);
        public const string LdapCredentialSectionName = $"{nameof(LdapConfiguration)}:{nameof(Credentials)}";

        public LdapCredentials Credentials { get; set; } = new LdapCredentials();
        public string? DisplayName { get; set; }
#if !NET10_0_OR_GREATER
        public UuiButtonLook? ButtonLook { get; set; }
        public UuiButtonColor? ButtonColor { get; set; }
#else
        public string? ButtonLook { get; set; }
        public string? ButtonColor { get; set; }
#endif
        public string? Icon { get; set; }
        public Dictionary<string, GroupBinding> GroupBindings { get; set; } = [];
        public bool? SetGroupsOnLogin { get; set; }
        public string[] AllowedGroupsFrontEnd { get; set; } = [];
        public string[] AllowedGroupsBackOffice { get; set; } = [];
        public string[]? DefaultGroups { get; set; }
        public bool? DenyLocalLogin { get; set; }
        public bool? AutoRedirectLoginToExternalProvider { get; set; }
        public bool AllowImpersonation { get; set; } = false;
        public string? TestUser { get; set; }
        public string RemoteSignOutPath { get; set; } = string.Empty;
        public string SignedOutRedirectUri { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string MetadataAddress { get; set; } = string.Empty;
        public int SettingsNodeId { get; set; }
        public bool EnableSSO { get; set; } = false;

        public class LdapCredentials
        {
            public string Instance { get; set; } = string.Empty;
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Realm { get; set; } = string.Empty;
            public string[] DCs { get; set; } = [];
        }

        public class GroupBinding
        {
            public string Name { get; set; } = string.Empty;
            public string Alias { get; set; } = string.Empty;
        }
    }
}
