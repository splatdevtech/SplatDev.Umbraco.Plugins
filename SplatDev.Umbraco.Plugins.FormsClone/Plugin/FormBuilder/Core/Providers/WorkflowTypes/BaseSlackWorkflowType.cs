using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Workflows;

namespace FormBuilder.Core.Providers.WorkflowTypes
{
    /// <summary>
    /// Provides common functionality for a     /// </summary>
    public abstract class BaseSlackWorkflowType : WorkflowType
    {
        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        protected BaseSlackWorkflowType()
        {
            Icon = "icon-chat-active";
            Group = "Services";
        }

        /// <summary>
        /// Generates the Slack notification message from the provided         /// </summary>
        protected virtual string GenerateNotificationMessage(Record record, Form form, bool escaped = true)
        {
            string notificationMessage = "Someone has posted the form '" + form.Name + "'." + Environment.NewLine;
            foreach (KeyValuePair<Guid, RecordField> recordField1 in record.RecordFields)
            {
                RecordField recordField2 = recordField1.Value;
                string str1 = recordField2.Field?.Caption ?? string.Empty;
                string str2 = recordField2.ValuesAsString(escaped);
                notificationMessage = notificationMessage + str1 + ": " + str2 + Environment.NewLine;
            }
            return notificationMessage;
        }
    }
}