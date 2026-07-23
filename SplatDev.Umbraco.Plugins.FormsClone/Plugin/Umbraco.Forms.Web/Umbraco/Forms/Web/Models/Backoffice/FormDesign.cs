
// Type: Umbraco.Forms.Web.Models.Backoffice.FormDesign
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Web.Models.Backoffice
{
  [Serializable]
  public class FormDesign : Form
  {
    public FormDesign()
      : this(new DefaultFormSettings())
    {
    }

    public FormDesign(DefaultFormSettings defaultSettings)
    {
      this.Pages = new List<Page>();
      this.StoreRecordsLocally = true;
      this.SetFromDefaults(defaultSettings);
    }

    [DataMember(Name = "formWorkflows")]
    public FormWorkflows FormWorkflows { get; set; } = new FormWorkflows();

    [DataMember(Name = "path")]
    public string Path { get; set; } = string.Empty;
  }
}
