
// Type: Umbraco.Forms.Web.Models.ManagementApi.Form.CopyFormModel
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73


#nullable enable
namespace Umbraco.Forms.Web.Models.ManagementApi.Form
{
  public class CopyFormModel
  {
    public string? NewName { get; set; }

    public bool CopyWorkflows { get; set; }

    public string? CopyToFolderId { get; set; }
  }
}
