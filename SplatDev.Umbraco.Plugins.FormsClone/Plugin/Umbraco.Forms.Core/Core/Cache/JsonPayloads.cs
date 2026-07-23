
// Type: Umbraco.Forms.Core.Cache.JsonPayloads
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Cache
{
  public class JsonPayloads
  {
    public class FolderPayload
    {
      public Folder? Folder { get; set; }

      public bool DeleteFolder { get; set; }
    }

    public class FormPayload
    {
      public Form? Form { get; set; }

      public bool DeleteForm { get; set; }
    }

    public class WorkflowPayload
    {
      public Workflow? Workflow { get; set; }

      public bool DeleteWorkflow { get; set; }
    }

    public class DataSourcePayload
    {
      public FormDataSource? DataSource { get; set; }

      public bool DeleteDataSource { get; set; }
    }

    public class PreValuePayload
    {
      public FieldPreValueSource? PreValue { get; set; }

      public bool DeletePreValue { get; set; }
    }
  }
}
