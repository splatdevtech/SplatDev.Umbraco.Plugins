using FormBuilder.Core.Persistence.Dtos;

using NPoco;

using System.Runtime.Serialization;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

using FormDto = FormBuilder.Core.Persistence.Dtos.FormDto;

namespace FormBuilder.Core.Persistence.Security
{
    [DataContract(Name = "userFormSecurity")]
    [TableName("FormBuilderUserFormSecurity")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class UserFormSecurity : FormSecurityBaseDto
    {
        [DataMember(Name = "id")]
        [PrimaryKeyColumn(AutoIncrement = true, Name = "PK_FBUserFormSecurity")]
        public int Id { get; set; }

        [DataMember(Name = "user")]
        [Length(50)]
        [Index(IndexTypes.Clustered)]
        public string User { get; set; } = string.Empty;

        [DataMember(Name = "form")]
        [ForeignKey(typeof(FormDto), Column = "Key")]
        public Guid Form { get; set; }

        public static UserFormSecurity Create() => new();
    }
}