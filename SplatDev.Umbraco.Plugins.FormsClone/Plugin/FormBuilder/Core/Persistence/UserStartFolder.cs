using FormBuilder.Core.Persistence.Dtos;

using NPoco;

using System.Runtime.Serialization;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace FormBuilder.Core.Persistence
{
    [DataContract(Name = "userStartFolder")]
    [TableName("FormBuilderUserStartFolders")]
    [PrimaryKey(["UserId", "FolderKey"])]
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