using FormBuilder.Core.Security.Interfaces;

namespace FormBuilder.Web.EntityPermissions
{
    /// <summary>
    /// Ensures the resource cannot be accessed if the current user does not have access to manage datasources.
    /// </summary>
    public class ManageDataSourceEntityHandler :
      EntityPermissionHandlerBase<ManageDataSourceEntityRequirement>
    {
        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public ManageDataSourceEntityHandler(IFormsSecurity formSecurity)
          : base(formSecurity)
        {
        }

        /// <inheritdoc />
        protected override bool HasPermission() => FormSecurity.CanCurrentUserManageDataSources();
    }
}