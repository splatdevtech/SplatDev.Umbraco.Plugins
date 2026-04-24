using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace SplatDev.Umbraco.Authorization.Ldap.Models
{
    public class AdGroups
    {
        public PrincipalSearchResult<Principal>? PrincipalGroups { get; set; }
        public List<NTAccount> NTAccountGroups { get; set; } = [];
        public IEnumerable<string>? Groups { get; set; }
    }
}
