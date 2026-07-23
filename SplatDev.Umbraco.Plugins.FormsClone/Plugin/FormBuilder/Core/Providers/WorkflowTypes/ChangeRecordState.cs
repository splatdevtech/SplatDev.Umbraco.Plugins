using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Workflows;

using Microsoft.Extensions.Logging;

using System.Text;

namespace FormBuilder.Core.Providers.WorkflowTypes
{
    /// <summary>
    /// Provides a     /// </summary>
    public class ChangeRecordState : WorkflowType
    {
        private readonly ILogger<ChangeRecordState> _logger;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public ChangeRecordState(ILogger<ChangeRecordState> logger)
        {
            Id = new Guid("4C40A092-0CB5-481D-96A7-A02D8E7CDB2F");
            Name = "Change Record State";
            Alias = "changeRecordState";
            Description = "Changes the state of the record being processed when it matches a word";
            Icon = "icon-autofill";
            Group = "Legacy";
            _logger = logger;
        }

        /// <summary>
        /// Gets or sets the comma seperated list of words to match.
        /// </summary>
        [Setting("Words", Description = "Comma seperated list of words to match", DisplayOrder = 10, IsMandatory = true, SupportsPlaceholders = true)]
        public virtual string Words { get; set; } = string.Empty;

        /// <summary>Gets or sets the action to apply (delete or approve).</summary>
        [Setting("Action", Description = "What to do if it matches", DisplayOrder = 20, IsMandatory = true, PreValues = "Delete Record,Approve Record,Reject Record", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string Action { get; set; } = string.Empty;

        /// <inheritdoc />
        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = [];
            if (string.IsNullOrEmpty(Words))
                exceptionList.Add(new Exception("'Words' setting has not been set"));
            if (string.IsNullOrEmpty(Action))
                exceptionList.Add(new Exception("'Action' setting has not been set"));
            return exceptionList;
        }

        /// <inheritdoc />
        public override Task<WorkflowExecutionStatus> ExecuteAsync(
          WorkflowExecutionContext context)
        {
            string[] source = Words.Split(',');
            string content = GetSubmissionContent(context.Record);
            Func<string, bool> predicate = s => content.Contains(s);
            if (!source.Any(predicate))
                return Task.FromResult(WorkflowExecutionStatus.Completed);
            string str;
            if (Action == "Delete Record")
            {
                str = "deleted";
                context.Record.State = FormState.Deleted;
            }
            else if (Action == "Reject Record")
            {
                str = "rejected";
                context.Record.State = FormState.Rejected;
            }
            else
            {
                str = "approved";
                context.Record.State = FormState.Approved;
            }
            _logger.LogDebug("The record with unique id {RecordId} that was submitted via the Form {FormName} with id {FormId} has been changed to {RecordState} state", context.Record.UniqueId, context.Form.Name, context.Form.Id, str);
            return Task.FromResult(WorkflowExecutionStatus.Completed);
        }

        private static string GetSubmissionContent(Record record)
        {
            StringBuilder stringBuilder = new();
            foreach (RecordField recordField in record.RecordFields.Values)
            {
                stringBuilder.Append(recordField.ValuesAsString());
                stringBuilder.Append(" ");
            }
            return stringBuilder.ToString();
        }
    }
}