using FormBuilder.Core.Persistence.Dtos;

using NPoco;

using System.Runtime.Serialization;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace FormBuilder.Core.Persistence.Security
{
    [DataContract(Name = "userSecurity")]
    [TableName("FormBuilderUserSecurity")]
    [PrimaryKey("User", AutoIncrement = false)]
    public class UserSecurity : SecurityBaseDto
    {
        [DataMember(Name = "user")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_FBUserSecurity")]
        [Length(50)]
        public string User { get; set; } = string.Empty;

        public static UserSecurity Create() => new();
    }
}