using FormBuilder.Core.Security.Interfaces;

namespace FormBuilder.Web.EntityPermissions
{
    /// <summary>
    /// Ensures the resource cannot be accessed if the current user does not have access to view or edit form entries.
    /// </summary>
    public class ViewRecordEntityHandler : EntityPermissionHandlerBase<ViewRecordEntityRequirement>
    {
        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public ViewRecordEntityHandler(IFormsSecurity formSecurity)
          : base(formSecurity)
        {
        }

        /// <inheritdoc />
        protected override bool HasPermission() => FormSecurity.CanCurrentUserViewEntries();
    }
}