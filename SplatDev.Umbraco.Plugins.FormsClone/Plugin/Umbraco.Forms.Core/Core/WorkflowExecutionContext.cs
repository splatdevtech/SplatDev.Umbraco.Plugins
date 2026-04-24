
// Type: Umbraco.Forms.Core.WorkflowExecutionContext
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections;
using System.Collections.Generic;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core
{
  public class WorkflowExecutionContext
  {
    public WorkflowExecutionContext(Record record, Form form, FormState state)
      : this(record, form, state, additionalData: record.GetAdditionalData())
    {
    }

    public WorkflowExecutionContext(
      Record record,
      Form form,
      FormState state,
      Hashtable pageElements)
      : this(record, form, state, pageElements, record.GetAdditionalData())
    {
    }

    public WorkflowExecutionContext(
      Record record,
      Form form,
      FormState state,
      Hashtable? pageElements = null,
      IDictionary<string, string?>? additionalData = null)
    {
      this.Record = record;
      this.State = state;
      this.Form = form;
      this.PageElements = pageElements;
      this.AdditionalData = additionalData ?? (IDictionary<string, string>) new Dictionary<string, string>();
    }

    public FormState State { get; }

    public Form Form { get; }

    public Record Record { get; }

    public Hashtable? PageElements { get; }

    public IDictionary<string, string?> AdditionalData { get; }
  }
}
