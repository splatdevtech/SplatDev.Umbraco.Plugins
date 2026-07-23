using FormBuilder.Core.Persistence.Dtos;

using NPoco;

using System.Runtime.Serialization;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace FormBuilder.Core.Persistence
{
    [DataContract(Name = "userGroupStartFolder")]
    [TableName("FormBuilderUserGroupStartFolders")]
    [PrimaryKey(["UserGroupId", "FolderKey"])]
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