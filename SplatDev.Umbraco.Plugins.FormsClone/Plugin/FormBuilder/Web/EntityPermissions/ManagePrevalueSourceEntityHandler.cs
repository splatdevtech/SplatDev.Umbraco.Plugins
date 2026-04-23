using FormBuilder.Core.Security.Interfaces;

namespace FormBuilder.Web.EntityPermissions
{
    /// <summary>
    /// Ensures the resource cannot be accessed if the current user does not have access to manage prevalue sources.
    /// </summary>
    public class ManagePrevalueSourceEntityHandler :
      EntityPermissionHandlerBase<ManagePrevalueSourceEntityRequirement>
    {
        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public ManagePrevalueSourceEntityHandler(IFormsSecurity formSecurity)
          : base(formSecurity)
        {
        }

        /// <inheritdoc />
        protected override bool HasPermission() => FormSecurity.CanCurrentUserManagePreValues();
    }
}