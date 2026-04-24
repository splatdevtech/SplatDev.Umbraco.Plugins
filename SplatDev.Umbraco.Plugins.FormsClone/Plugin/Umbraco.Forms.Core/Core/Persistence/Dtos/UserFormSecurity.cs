
// Type: Umbraco.Forms.Core.Persistence.Dtos.UserFormSecurity
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;

using System.Runtime.Serialization;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Dtos
{
    [DataContract(Name = "userFormSecurity")]
    [TableName("UFUserFormSecurity")]
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
