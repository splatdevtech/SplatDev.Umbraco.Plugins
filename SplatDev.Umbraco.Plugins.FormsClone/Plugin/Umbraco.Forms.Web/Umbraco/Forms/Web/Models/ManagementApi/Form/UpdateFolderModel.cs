
// Type: Umbraco.Forms.Web.Models.ManagementApi.Form.UpdateFolderModel
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System.ComponentModel.DataAnnotations;


#nullable enable
namespace Umbraco.Forms.Web.Models.ManagementApi.Form
{
  public class UpdateFolderModel
  {
    [Required]
    public string Name { get; set; } = string.Empty;
  }
}
