
// Type: Umbraco.Forms.Core.Providers.WorkflowTypes.BaseSlackWorkflowType
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Providers.WorkflowTypes
{
    public abstract class BaseSlackWorkflowType : WorkflowType
    {
        protected BaseSlackWorkflowType()
        {
            this.Icon = "icon-chat-active";
            this.Group = "Services";
        }

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
