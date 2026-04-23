
// Type: Umbraco.Forms.Core.Configuration.SecuritySettings
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Umbraco.Forms.Core.Configuration
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

    public string[] GetDisallowedFileUploadExtensionsAsArray() => this.GetFileUploadExtensionsAsArray(this.DisallowedFileUploadExtensions);

    public string[] GetAllowedFileUploadExtensionsAsArray() => this.GetFileUploadExtensionsAsArray(this.AllowedFileUploadExtensions);

    private string[] GetFileUploadExtensionsAsArray(string fileExtensions) => ((IEnumerable<string>) fileExtensions.Split(',', StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (x => "." + x.ToLowerInvariant())).ToArray<string>();
  }
}
