using FormBuilder.Core.Security.Interfaces;

using Microsoft.AspNetCore.Authorization;

using Umbraco.Cms.Api.Management.Security.Authorization;

namespace FormBuilder.Web.EntityPermissions
{
    /// <summary>
    /// Base class for handlers verifying access to operations on an entity.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public abstract class EntityPermissionHandlerBase<T>(IFormsSecurity formSecurity) :
      MustSatisfyRequirementAuthorizationHandler<T>
      where T : IAuthorizationRequirement
    {
        /// <summary>
        /// Gets the         /// </summary>
        protected IFormsSecurity FormSecurity { get; } = formSecurity;

        /// <inheritdoc />
        protected override Task<bool> IsAuthorized(
          AuthorizationHandlerContext context,
          T requirement)
        {
            return Task.FromResult(HasPermission());
        }

        /// <summary>
        /// Checks if the current user has the necessary permission for managing entities.
        /// </summary>
        protected abstract bool HasPermission();
    }
}