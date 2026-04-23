using FormBuilder.Core.Enums;
using FormBuilder.Core.Extensions;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;

using System.Collections;

namespace FormBuilder.Core.Workflows
{
    public class WorkflowExecutionContext(
      Record record,
      Form form,
      FormState state,
      Hashtable? pageElements = null,
      IDictionary<string, string?>? additionalData = null)
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

        public FormState State { get; } = state;

        public Form Form { get; } = form;

        public Record Record { get; } = record;

        public Hashtable? PageElements { get; } = pageElements;

        public IDictionary<string, string?> AdditionalData { get; } = additionalData ?? new Dictionary<string, string?>();
    }
}