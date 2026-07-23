using FormBuilder.Core.Persistence.Security;

namespace FormBuilder.Core.Storage.Interfaces
{
    public interface IUserFormSecurityStorage
    {
        List<UserFormSecurity> GetAllUserFormSecurity(string userId);

        UserFormSecurity? GetUserFormSecurity(string userId, Guid form);

        UserFormSecurity InsertUserFormSecurity(UserFormSecurity formSecurity);

        List<UserFormSecurity> GetUserFormSecurityForAllUsers(Guid form);

        bool DeleteUserFormSecurity(UserFormSecurity formSecurity);

        void DeleteAllUserFormSecurityForForm(Guid form);

        void DeleteAllUserFormSecurityForUser(int userId);

        UserFormSecurity UpdateUserFormSecurity(UserFormSecurity formSecurity);
    }
}