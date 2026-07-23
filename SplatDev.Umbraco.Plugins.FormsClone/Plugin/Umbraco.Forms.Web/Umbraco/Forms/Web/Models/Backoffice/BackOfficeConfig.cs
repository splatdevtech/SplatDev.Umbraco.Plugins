
// Type: Umbraco.Forms.Web.Models.Backoffice.BackOfficeConfig
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Web.Models.Backoffice
{
  [DataContract]
  public class BackOfficeConfig
  {
    [DataMember(Name = "maxNumberOfColumnsInFormGroup")]
    public int MaxNumberOfColumnsInFormGroup { get; set; }

    [DataMember(Name = "manageSecurityWithUserGroups")]
    public bool ManageSecurityWithUserGroups { get; set; }

    [DataMember(Name = "scheduledRecordDeletionEnabled")]
    public bool ScheduledRecordDeletionEnabled { get; set; }

    [DataMember(Name = "mandatoryFieldsetLegends")]
    public bool MandatoryFieldsetLegends { get; set; }

    [DataMember(Name = "disallowedFileUploadExtensions")]
    public string DisallowedFileUploadExtensions { get; set; } = string.Empty;

    [DataMember(Name = "allowedFileUploadExtensions")]
    public string AllowedFileUploadExtensions { get; set; } = string.Empty;

    [DataMember(Name = "enableMultiPageFormSettings")]
    public bool EnableMultiPageFormSettings { get; set; }

    [DataMember(Name = "enableAdvancedValidationRules")]
    public bool EnableAdvancedValidationRules { get; set; }
  }
}
