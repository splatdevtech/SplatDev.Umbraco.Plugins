using FormBuilder.Core.Persistence.Security;

using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>
    /// Defines a representation of form security defined for a user group.
    /// </summary>
    [DataContract(Name = "formSecurityForGroup")]
    [Serializable]
    public class FormSecurityForGroup
    {
        /// <summary>Gets or sets the user group key.</summary>
        [DataMember(Name = "key")]
        public Guid Key { get; set; }

        /// <summary>Gets or sets the user group name.</summary>
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets the unique ID for the group (with the field name required for front-end rendering).
        /// </summary>
        [DataMember(Name = "unique")]
        public Guid Unique => Key;

        /// <summary>
        /// Gets the form entity type (required for front-end rendering).
        /// </summary>
        [DataMember(Name = "entityType")]
        public static string EntityType => "forms-security-user-group";

        /// <summary>Gets or sets the group level security.</summary>
        [DataMember(Name = "userGroupSecurity")]
        public UserGroupSecurity UserGroupSecurity { get; set; } = new UserGroupSecurity();

        /// <summary>Gets or sets the group start folder keys.</summary>
        [DataMember(Name = "startFolders")]
        public List<Guid> StartFolderIds { get; set; } = [];

        /// <summary>Gets or sets the per-form level security.</summary>
        [DataMember(Name = "formsSecurity")]
        public List<UserGroupFormSecurity> FormsSecurity { get; set; } = [];
    }
}