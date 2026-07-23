using FormBuilder.Core.Security.Interfaces;

namespace FormBuilder.Web.EntityPermissions
{
    /// <summary>
    /// Ensures the resource cannot be accessed if the current user does not have access to manage forms.
    /// </summary>
    public class ManageFormEntityHandler : EntityPermissionHandlerBase<ManageFormEntityRequirement>
    {
        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public ManageFormEntityHandler(IFormsSecurity formSecurity)
          : base(formSecurity)
        {
        }

        /// <inheritdoc />
        protected override bool HasPermission() => FormSecurity.CanCurrentUserManageForms();
    }
}