using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Membership;
using UmbracoCms.Plugins.MemberGroups.Interfaces;
using UmbracoCms.Plugins.MemberGroups.Models;
using MemberGroup = UmbracoCms.Plugins.MemberGroups.Models.MemberGroup;

namespace UmbracoCms.Plugins.MemberGroups.Services
{
    public interface IMemberGroupsService
    {
        IMember? CurrentMember { get; }
        int MinPasswordLength { get; set; }

        void AddToGroup(string email, string group);
        int AssignUserToGroup(string username, string groupName);
        string ChangeUserPassword(MemberUser userToChange, string currentPassword);
        MemberGroup CreateGroup(MemberGroup groupToCreate);
        IMemberUser CreateUser(MemberUser userToCreate);
        IMember CreateMember(IMember member);
        void DeleteMember(IMember member);
        void DeleteMemberGroup(string groupName);
        void DeleteMemberType(IMemberType memberType);
        void DisableUser(string username);
        void DisableUser(int userId);
        IUserGroup EditGroup(string oldGroupName, MemberGroup newGroup);
        string EnableUser(string username);
        string EnableUser(int userId);
        IMember? GetByEmail(string email);
        IMember? GetByUsername(string username);
        IMember? GetMemberById(int memberId);
        IMember? GetMemberById(string memberId);
        IEnumerable<IMemberGroup> GetMemberGroups();
        IEnumerable<IMemberType> GetMemberTypes();
        IUserGroup? GetUserGroup(string alias);
        string GetUserName(int userId);
        string GetUserName(string userId);
        bool GroupExists(MemberGroup group);
        bool MemberExists(string emailAddress);
        bool MemberGroupExists(string groupName);
        bool MemberTypeExists(string memberTypeAlias);
        int RegisterMember(string memberName, string emailAddress, string memberPassword, string memberTypeAlias, string memberGroupName, string username = "");
        string ResetUserPassword(string username);
        void SaveMember(IMember member);
        void SaveMemberGroup(string groupName);
        void SaveMemberType(string parentGroupAlias, string memberTypeAlias, string memberTypeName = "", string description = "", string icon = "icon-user");
        void SaveMemberType(IMemberType memberType);
        void SavePassword(IMember member, string newPassword);
        void UpdateMemberPassword(IMember member, string password);
        void UpdateUser(MemberUser userToUpdate);
        bool UserExists(MemberUser user);
        bool ValidateMember(string email, string validation);
        bool ValidatePasswordLength(MemberUser user);
        bool ValidationMatches(string email, string validation);
        IUser? GetUserById(int userId);
    }
}
