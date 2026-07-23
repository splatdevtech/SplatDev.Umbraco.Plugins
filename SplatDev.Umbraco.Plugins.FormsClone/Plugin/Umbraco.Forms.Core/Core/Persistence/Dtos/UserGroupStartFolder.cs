
// Type: Umbraco.Forms.Core.Persistence.Dtos.UserGroupStartFolder
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;

using System.Runtime.Serialization;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Forms.Core.Persistence.Dtos
{
    [DataContract(Name = "userGroupStartFolder")]
    [TableName("UFUserGroupStartFolders")]
    [PrimaryKey(new string[] { "UserGroupId", "FolderKey" })]
    public class UserGroupStartFolder
    {
        [Column(Name = "UserGroupId")]
        [Index(IndexTypes.NonClustered)]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_FBUserGroupStartFolders", OnColumns = "UserGroupId, FolderKey")]
        public int UserGroupId { get; set; }

        [Column(Name = "FolderKey")]
        [ForeignKey(typeof(FolderDto), Column = "Key")]
        [Index(IndexTypes.NonClustered)]
        public Guid FolderKey { get; set; }
    }
}
