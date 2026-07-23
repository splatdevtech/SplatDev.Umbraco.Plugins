
// Type: Umbraco.Forms.Core.Persistence.Dtos.UserSecurity
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;
using System.Runtime.Serialization;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Dtos
{
  [DataContract(Name = "userSecurity")]
  [TableName("UFUserSecurity")]
  [PrimaryKey("User", AutoIncrement = false)]
  public class UserSecurity : SecurityBaseDto
  {
    [DataMember(Name = "user")]
    [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_UFUserSecurity")]
    [Length(50)]
    public string User { get; set; } = string.Empty;

    public static UserSecurity Create() => new UserSecurity();
  }
}
