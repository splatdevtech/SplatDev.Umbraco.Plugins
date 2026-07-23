using Umbraco.Cms.Core.Models.Membership;

namespace FormBuilder.Core.Security.Interfaces
{
    public interface IFormsSecurity
    {
        bool HasAccessToForm(Guid form);

        bool HasAccessToForm(Guid form, IUser user);

        bool CanCurrentUserManageWorkflows();

        bool CanCurrentUserManageForms();

        bool CanUserManageForms(IUser user);

        bool CanCurrentUserManageDataSources();

        bool CanCurrentUserManagePreValues();

        bool CanCurrentUserViewEntries();

        bool CanUserViewEntries(IUser user);

        bool CanCurrentUserEditEntries();

        bool CanCurrentUserDeleteEntries();

        IEnumerable<Guid> FilterFormIdsForCurrentUser(IEnumerable<Guid> formIds);

        IEnumerable<Guid> FilterFormIdsForUser(IEnumerable<Guid> formIds, IUser user);

        IEnumerable<Guid> GetStartFolderKeysForCurrentUser();

        IEnumerable<Guid> GetStartFolderKeysForUser(IUser user);
    }
}