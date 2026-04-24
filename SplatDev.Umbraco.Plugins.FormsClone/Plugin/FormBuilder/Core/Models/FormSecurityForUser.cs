using FormBuilder.Core.Persistence.Security;

using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>
    /// Defines a representation of form security defined for a user.
    /// </summary>
    [DataContract(Name = "formSecurityForUser")]
    [Serializable]
    public class FormSecurityForUser
    {
        /// <summary>Gets or sets the user key.</summary>
        [DataMember(Name = "key")]
        public Guid Key { get; set; }

        /// <summary>Gets or sets the user name.</summary>
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets the unique ID for the user (with the field name required for front-end rendering).
        /// </summary>
        [DataMember(Name = "unique")]
        public Guid Unique => Key;

        /// <summary>
        /// Gets the form entity type (required for front-end rendering).
        /// </summary>
        [DataMember(Name = "entityType")]
        public static string EntityType => "forms-security-user";

        /// <summary>Gets or sets the user level security.</summary>
        [DataMember(Name = "userSecurity")]
        public UserSecurity UserSecurity { get; set; } = new UserSecurity();

        /// <summary>Gets or sets the user start folder keys.</summary>
        [DataMember(Name = "startFolders")]
        public List<Guid> StartFolderIds { get; set; } = [];

        /// <summary>Gets or sets the per-form level security.</summary>
        [DataMember(Name = "formsSecurity")]
        public List<UserFormSecurity> FormsSecurity { get; set; } = [];
    }
}