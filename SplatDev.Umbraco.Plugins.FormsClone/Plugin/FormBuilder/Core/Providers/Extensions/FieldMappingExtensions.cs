using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Providers.Models;
using FormBuilder.Core.Workflows;

using Microsoft.Extensions.Logging;

using System.Collections.Specialized;

namespace FormBuilder.Core.Providers.Extensions
{
    public static class FieldMappingExtensions
    {
        public static NameValueCollection ToNameValueCollection(
          this List<FieldMapping> mappings,
          WorkflowExecutionContext context,
          ILogger logger,
          string workflowName)
        {
            NameValueCollection nameValueCollection = [];
            foreach (FieldMapping mapping in mappings)
            {
                string alias = mapping.Alias;
                if (!string.IsNullOrEmpty(mapping.StaticValue))
                {
                    string staticValue = mapping.StaticValue;
                    nameValueCollection.Add(alias, staticValue);
                }
                else
                {
                    if (Guid.TryParse(mapping.Value, out Guid result))
                    {
                        RecordField? recordField = context.Record.GetRecordField(result);
                        if (recordField is not null)
                        {
                            string str = recordField.ValuesAsString(false);
                            nameValueCollection.Add(alias, str);
                        }
                        else
                            logger.LogWarning("Workflow {WorkflowName}: The field mapping with alias '{FieldMappingAlias}' did not match any record fields. This is probably caused by the record field being marked as sensitive and the workflow has been set not to include sensitive data.", workflowName, mapping.Alias);
                    }
                    else
                        logger.LogWarning("Workflow {WorkflowName}: The field mapping with alias '{FieldMappingAlias}' did not contain a valid field ID.", workflowName, mapping.Alias);
                }
            }
            return nameValueCollection;
        }
    }
}