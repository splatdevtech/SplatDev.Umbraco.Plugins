
// Type: Umbraco.Forms.Web.Models.Backoffice.DataTypeDetail
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;


#nullable enable
namespace Umbraco.Forms.Web.Models.Backoffice
{
  public class DataTypeDetail
  {
    public int Id { get; set; }

    public Guid Key { get; set; }

    public string Name { get; set; } = string.Empty;

    public IDictionary<string, object> ConfigurationData { get; set; } = (IDictionary<string, object>) new Dictionary<string, object>();
  }
}
