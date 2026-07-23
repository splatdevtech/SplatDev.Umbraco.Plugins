
// Type: Umbraco.Forms.Web.Models.ManagementApi.Form.Tree.FormTreeItemResponseModel
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Umbraco.Cms.Api.Management.ViewModels.Tree;


#nullable enable
namespace Umbraco.Forms.Web.Models.ManagementApi.Form.Tree
{
  public class FormTreeItemResponseModel : FolderTreeItemResponseModel
  {
    public string Path { get; set; } = string.Empty;
  }
}
