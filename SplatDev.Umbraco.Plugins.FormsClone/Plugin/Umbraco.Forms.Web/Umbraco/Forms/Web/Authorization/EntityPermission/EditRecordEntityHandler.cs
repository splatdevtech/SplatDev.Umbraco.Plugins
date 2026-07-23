
// Type: Umbraco.Forms.Web.Authorization.EntityPermission.EditRecordEntityHandler
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Umbraco.Forms.Core.Security;


#nullable enable
namespace Umbraco.Forms.Web.Authorization.EntityPermission
{
  public class EditRecordEntityHandler : EntityPermissionHandlerBase<EditRecordEntityRequirement>
  {
    public EditRecordEntityHandler(IFormsSecurity formSecurity)
      : base(formSecurity)
    {
    }

    protected override bool HasPermission() => this.FormSecurity.CanCurrentUserViewEntries();
  }
}
