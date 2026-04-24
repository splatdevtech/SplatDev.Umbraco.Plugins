
// Type: Umbraco.Forms.Web.Models.Backoffice.DataSourceWizard
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Web.Models.Backoffice
{
  [DataContract(Name = "dataSourceWizard")]
  [Serializable]
  public class DataSourceWizard
  {
    [DataMember(Name = "dataSourceGuid")]
    public Guid DataSourceGuid { get; set; }

    [DataMember(Name = "formName")]
    public string FormName { get; set; } = string.Empty;

    [DataMember(Name = "mappings")]
    public IEnumerable<DataSourceWizardFieldMapping> Mappings { get; set; } = Enumerable.Empty<DataSourceWizardFieldMapping>();
  }
}
