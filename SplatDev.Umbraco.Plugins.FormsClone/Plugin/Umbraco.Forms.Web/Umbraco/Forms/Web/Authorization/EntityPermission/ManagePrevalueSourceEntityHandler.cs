
// Type: Umbraco.Forms.Web.Authorization.EntityPermission.ManagePrevalueSourceEntityHandler
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Umbraco.Forms.Core.Security;


#nullable enable
namespace Umbraco.Forms.Web.Authorization.EntityPermission
{
  public class ManagePrevalueSourceEntityHandler : 
    EntityPermissionHandlerBase<ManagePrevalueSourceEntityRequirement>
  {
    public ManagePrevalueSourceEntityHandler(IFormsSecurity formSecurity)
      : base(formSecurity)
    {
    }

    protected override bool HasPermission() => this.FormSecurity.CanCurrentUserManagePreValues();
  }
}
