using FormBuilder.Core.Persistence.Security;

namespace FormBuilder.Core.Storage.Interfaces
{
    public interface IUserSecurityStorage
    {
        List<UserSecurity> GetAllUserSecurity();

        UserSecurity? GetUserSecurity(int userId);

        UserSecurity InsertUserSecurity(UserSecurity usersecurity);

        bool DeleteUserSecurity(UserSecurity userSecurity);

        UserSecurity UpdateUserSecurity(UserSecurity userSecurity);
    }
}