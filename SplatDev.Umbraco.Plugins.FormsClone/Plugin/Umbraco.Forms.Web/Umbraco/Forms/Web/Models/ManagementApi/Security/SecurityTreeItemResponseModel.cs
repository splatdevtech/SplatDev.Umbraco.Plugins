
// Type: Umbraco.Forms.Web.Models.ManagementApi.Security.SecurityTreeItemResponseModel
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Umbraco.Cms.Api.Management.ViewModels.Tree;

namespace Umbraco.Forms.Web.Models.ManagementApi.Security
{
  public class SecurityTreeItemResponseModel : FolderTreeItemResponseModel
  {
    public bool IsGroup { get; set; }
  }
}
