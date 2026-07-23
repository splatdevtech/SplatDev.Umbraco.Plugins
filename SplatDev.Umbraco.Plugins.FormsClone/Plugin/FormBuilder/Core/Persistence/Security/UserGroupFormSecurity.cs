using FormBuilder.Core.Persistence.Dtos;

using NPoco;

using System.Runtime.Serialization;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace FormBuilder.Core.Persistence.Security
{
    [DataContract(Name = "userGroupFormSecurity")]
    [TableName("FormBuilderUserGroupFormSecurity")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class UserGroupFormSecurity : FormSecurityBaseDto
    {
        [DataMember(Name = "id")]
        [PrimaryKeyColumn(AutoIncrement = true, Name = "PK_FBUserGroupFormSecurity")]
        public int Id { get; set; }

        [DataMember(Name = "userGroupId")]
        [Index(IndexTypes.UniqueNonClustered)]
        public int UserGroupId { get; set; }

        [DataMember(Name = "form")]
        [ForeignKey(typeof(FormDto), Column = "Key")]
        public Guid Form { get; set; }

        public static UserGroupFormSecurity Create() => new();
    }
}