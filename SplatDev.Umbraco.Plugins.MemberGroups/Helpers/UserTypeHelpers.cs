using SplatDev.Umbraco.Plugins.MemberGroups.Enums;

namespace SplatDev.Umbraco.Plugins.MemberGroups.Helpers
{
    public static class UserTypeHelpers
    {
        public static string GroupToString(this UserTypes group)
        {
            return group switch
            {
                UserTypes.Administrator => "admin",
                UserTypes.Writer => "writer",
                UserTypes.Editor => "editor",
                UserTypes.Translator => "translator",
                UserTypes.SensitiveData => "sensitiveData",
                _ => "editor"
            };
        }
    }
}
