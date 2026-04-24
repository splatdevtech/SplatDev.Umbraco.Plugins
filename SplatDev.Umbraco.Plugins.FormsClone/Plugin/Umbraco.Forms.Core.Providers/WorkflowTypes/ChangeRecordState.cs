
// Type: Umbraco.Forms.Core.Providers.WorkflowTypes.ChangeRecordState
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.Extensions.Logging;

using System.Text;

using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Providers.WorkflowTypes
{
    public class ChangeRecordState : WorkflowType
    {
        private readonly ILogger<ChangeRecordState> _logger;

        public ChangeRecordState(ILogger<ChangeRecordState> logger)
        {
            this.Id = new Guid("4C40A092-0CB5-481D-96A7-A02D8E7CDB2F");
            this.Name = "Change Record State";
            this.Alias = "changeRecordState";
            this.Description = "Changes the state of the record being processed when it matches a word";
            this.Icon = "icon-autofill";
            this.Group = "Legacy";
            this._logger = logger;
        }

        [Setting("Words", Description = "Comma seperated list of words to match", DisplayOrder = 10, IsMandatory = true, SupportsPlaceholders = true)]
        public virtual string Words { get; set; } = string.Empty;

        [Setting("Action", Description = "What to do if it matches", DisplayOrder = 20, IsMandatory = true, PreValues = "Delete Record,Approve Record,Reject Record", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string Action { get; set; } = string.Empty;

        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = new List<Exception>();
            if (string.IsNullOrEmpty(this.Words))
                exceptionList.Add(new Exception("'Words' setting has not been set"));
            if (string.IsNullOrEmpty(this.Action))
                exceptionList.Add(new Exception("'Action' setting has not been set"));
            return exceptionList;
        }

        public override Task<WorkflowExecutionStatus> ExecuteAsync(
          WorkflowExecutionContext context)
        {
            string[] source = this.Words.Split(',');
            string content = ChangeRecordState.GetSubmissionContent(context.Record);
            Func<string, bool> predicate = s => content.Contains(s);
            if (!source.Any<string>(predicate))
                return Task.FromResult<WorkflowExecutionStatus>(WorkflowExecutionStatus.Completed);
            string str;
            if (this.Action == "Delete Record")
            {
                str = "deleted";
                context.Record.State = FormState.Deleted;
            }
            else if (this.Action == "Reject Record")
            {
                str = "rejected";
                context.Record.State = FormState.Rejected;
            }
            else
            {
                str = "approved";
                context.Record.State = FormState.Approved;
            }
            this._logger.LogDebug("The record with unique id {RecordId} that was submitted via the Form {FormName} with id {FormId} has been changed to {RecordState} state", context.Record.UniqueId, context.Form.Name, context.Form.Id, str);
            return Task.FromResult<WorkflowExecutionStatus>(WorkflowExecutionStatus.Completed);
        }

        private static string GetSubmissionContent(Record record)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (RecordField recordField in record.RecordFields.Values)
            {
                stringBuilder.Append(recordField.ValuesAsString());
                stringBuilder.Append(" ");
            }
            return stringBuilder.ToString();
        }
    }
}
