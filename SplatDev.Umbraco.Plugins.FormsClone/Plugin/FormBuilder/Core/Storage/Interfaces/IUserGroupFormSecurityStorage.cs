using FormBuilder.Core.Persistence.Security;

namespace FormBuilder.Core.Storage.Interfaces
{
    public interface IUserGroupFormSecurityStorage
    {
        List<UserGroupFormSecurity> GetAllUserGroupFormSecurity(int groupId);

        List<UserGroupFormSecurity> GetAllUserGroupFormSecurity(
          params int[] groupIds);

        List<UserGroupFormSecurity> GetAllUserGroupFormSecurity(
          Guid form,
          params int[] groupIds);

        UserGroupFormSecurity? GetUserGroupFormSecurity(int groupId, Guid form);

        UserGroupFormSecurity InsertUserGroupFormSecurity(
          UserGroupFormSecurity formSecurity);

        List<UserGroupFormSecurity> GetUserGroupFormSecurityForAllUsers(
          Guid form);

        bool DeleteUserGroupFormSecurity(UserGroupFormSecurity formSecurity);

        void DeleteAllUserGroupFormSecurityForForm(Guid form);

        void DeleteAllUserGroupFormSecurityForUserGroup(int userGroupId);

        UserGroupFormSecurity UpdateUserGroupFormSecurity(
          UserGroupFormSecurity formSecurity);
    }
}