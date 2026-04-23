
// Type: Umbraco.Forms.Core.Persistence.Dtos.UserStartFolder
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;

using System.Runtime.Serialization;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Forms.Core.Persistence.Dtos
{
    [DataContract(Name = "userStartFolder")]
    [TableName("UFUserStartFolders")]
    [PrimaryKey(new string[] { "UserId", "FolderKey" })]
    public class UserStartFolder
    {
        [Column(Name = "UserId")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_FBUserStartFolders", OnColumns = "UserId, FolderKey")]
        [Index(IndexTypes.NonClustered)]
        public int UserId { get; set; }

        [Column(Name = "FolderKey")]
        [ForeignKey(typeof(FolderDto), Column = "Key")]
        [Index(IndexTypes.NonClustered)]
        public Guid FolderKey { get; set; }
    }
}
