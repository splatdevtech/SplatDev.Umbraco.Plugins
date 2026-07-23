
// Type: Umbraco.Forms.Web.Models.ManagementApi.Form.CopyFormWorkflowsModel
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Umbraco.Forms.Web.Models.ManagementApi.Form
{
  public class CopyFormWorkflowsModel
  {
    public Guid DestinationId { get; set; }

    public IEnumerable<Guid> WorkflowIds { get; set; } = Enumerable.Empty<Guid>();
  }
}
