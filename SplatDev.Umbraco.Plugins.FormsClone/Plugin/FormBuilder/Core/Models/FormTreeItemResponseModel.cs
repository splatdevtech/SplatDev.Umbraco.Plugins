using Umbraco.Cms.Api.Management.ViewModels.Tree;

namespace FormBuilder.Core.Models
{
    public class FormTreeItemResponseModel : FolderTreeItemResponseModel
    {
        public string Path { get; set; } = string.Empty;
    }
}