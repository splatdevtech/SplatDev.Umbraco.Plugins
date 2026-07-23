using FormBuilder.Core.Persistence.Security;

namespace FormBuilder.Core.Storage.Interfaces
{
    public interface IUserGroupSecurityStorage
    {
        List<UserGroupSecurity> GetAllUserGroupSecurity(params int[] groupIds);

        UserGroupSecurity? GetUserGroupSecurity(int groupId);

        UserGroupSecurity InsertUserGroupSecurity(UserGroupSecurity usersecurity);

        bool DeleteUserGroupSecurity(UserGroupSecurity userSecurity);

        UserGroupSecurity UpdateUserGroupSecurity(UserGroupSecurity userSecurity);
    }
}