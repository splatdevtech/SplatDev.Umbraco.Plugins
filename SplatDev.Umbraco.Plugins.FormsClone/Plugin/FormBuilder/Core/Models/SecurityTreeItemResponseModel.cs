using Umbraco.Cms.Api.Management.ViewModels.Tree;

namespace FormBuilder.Core.Models
{
    public class SecurityTreeItemResponseModel : FolderTreeItemResponseModel
    {
        public bool IsGroup { get; set; }
    }
}