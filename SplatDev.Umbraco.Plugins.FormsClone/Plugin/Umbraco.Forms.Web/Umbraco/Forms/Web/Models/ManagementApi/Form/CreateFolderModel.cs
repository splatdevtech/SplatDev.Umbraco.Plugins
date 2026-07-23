
// Type: Umbraco.Forms.Web.Models.ManagementApi.Form.CreateFolderModel
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.ComponentModel.DataAnnotations;


#nullable enable
namespace Umbraco.Forms.Web.Models.ManagementApi.Form
{
  public class CreateFolderModel
  {
    [Required]
    public Guid Id { get; set; }

    public Guid? ParentId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
  }
}
