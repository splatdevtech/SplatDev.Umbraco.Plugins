using FormBuilder.Core.Models;

namespace FormBuilder.Core.Configuration
{
    public class SecuritySettings
    {
        public string DisallowedFileUploadExtensions { get; set; } = "config,exe,dll,asp,aspx,js";

        public string AllowedFileUploadExtensions { get; set; } = string.Empty;

        public bool EnableAntiForgeryToken { get; set; } = true;

        public bool SavePlainTextPasswords { get; set; }

        public bool DisableFileUploadAccessProtection { get; set; }

        public bool ManageSecurityWithUserGroups { get; set; }

        public string GrantAccessToNewFormsForUserGroups { get; set; } = "admin,editor";

        public FormAccess DefaultUserAccessToNewForms { get; set; }

        public string? FormsApiKey { get; set; }

        public bool EnableAntiForgeryTokenForFormsApi { get; set; } = true;

        public string[] GetDisallowedFileUploadExtensionsAsArray() => GetFileUploadExtensionsAsArray(DisallowedFileUploadExtensions);

        public string[] GetAllowedFileUploadExtensionsAsArray() => GetFileUploadExtensionsAsArray(AllowedFileUploadExtensions);

        private static string[] GetFileUploadExtensionsAsArray(string fileExtensions) => [.. fileExtensions.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => "." + x.ToLowerInvariant())];
    }
}