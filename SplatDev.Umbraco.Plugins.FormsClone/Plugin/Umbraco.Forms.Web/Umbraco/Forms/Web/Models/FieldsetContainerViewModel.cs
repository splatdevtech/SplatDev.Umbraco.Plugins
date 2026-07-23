
// Type: Umbraco.Forms.Web.Models.FieldsetContainerViewModel
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;


#nullable enable
namespace Umbraco.Forms.Web.Models
{
  [Serializable]
  public class FieldsetContainerViewModel
  {
    public string Caption { get; set; } = string.Empty;

    public int Width { get; set; }

    public IList<FieldViewModel> Fields { get; set; } = (IList<FieldViewModel>) new List<FieldViewModel>();
  }
}
