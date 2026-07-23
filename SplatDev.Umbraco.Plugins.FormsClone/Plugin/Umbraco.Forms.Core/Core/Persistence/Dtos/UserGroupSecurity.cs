
// Type: Umbraco.Forms.Core.Persistence.Dtos.UserGroupSecurity
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;
using System.Runtime.Serialization;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Dtos
{
  [DataContract(Name = "userGroupSecurity")]
  [TableName("UFUserGroupSecurity")]
  [PrimaryKey("UserGroupId", AutoIncrement = false)]
  public class UserGroupSecurity : SecurityBaseDto
  {
    [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_UFUserGroupSecurity")]
    [DataMember(Name = "userGroupId")]
    public int UserGroupId { get; set; }

    public static UserGroupSecurity Create() => new UserGroupSecurity();
  }
}
