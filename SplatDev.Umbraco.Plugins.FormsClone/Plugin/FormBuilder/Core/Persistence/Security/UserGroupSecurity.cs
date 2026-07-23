using FormBuilder.Core.Persistence.Dtos;

using NPoco;

using System.Runtime.Serialization;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace FormBuilder.Core.Persistence.Security
{
    [DataContract(Name = "userGroupSecurity")]
    [TableName("FormBuilderUserGroupSecurity")]
    [PrimaryKey("UserGroupId", AutoIncrement = false)]
    public class UserGroupSecurity : SecurityBaseDto
    {
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_FBUserGroupSecurity")]
        [DataMember(Name = "userGroupId")]
        public int UserGroupId { get; set; }

        public static UserGroupSecurity Create() => new();
    }
}