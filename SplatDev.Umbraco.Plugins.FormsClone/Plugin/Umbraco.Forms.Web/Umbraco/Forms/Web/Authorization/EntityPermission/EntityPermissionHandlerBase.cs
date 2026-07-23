
// Type: Umbraco.Forms.Web.Authorization.EntityPermission.EntityPermissionHandlerBase`1
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Umbraco.Cms.Api.Management.Security.Authorization;
using Umbraco.Forms.Core.Security;


#nullable enable
namespace Umbraco.Forms.Web.Authorization.EntityPermission
{
  public abstract class EntityPermissionHandlerBase<T> : 
    MustSatisfyRequirementAuthorizationHandler<T>
    where T : IAuthorizationRequirement
  {
    protected EntityPermissionHandlerBase(IFormsSecurity formSecurity) => this.FormSecurity = formSecurity;

    protected IFormsSecurity FormSecurity { get; }

    protected override Task<bool> IsAuthorized(
      AuthorizationHandlerContext context,
      T requirement)
    {
      return Task.FromResult<bool>(this.HasPermission());
    }

    protected abstract bool HasPermission();
  }
}
