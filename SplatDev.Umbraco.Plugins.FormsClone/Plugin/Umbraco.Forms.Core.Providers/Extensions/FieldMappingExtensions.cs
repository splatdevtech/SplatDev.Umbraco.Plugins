
// Type: Umbraco.Forms.Core.Providers.Extensions.FieldMappingExtensions
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Providers.Models;


#nullable enable
namespace Umbraco.Forms.Core.Providers.Extensions
{
  public static class FieldMappingExtensions
  {
    public static NameValueCollection ToNameValueCollection(
      this List<FieldMapping> mappings,
      WorkflowExecutionContext context,
      ILogger logger,
      string workflowName)
    {
      NameValueCollection nameValueCollection = new NameValueCollection();
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
          Guid result;
          if (Guid.TryParse(mapping.Value, out result))
          {
            RecordField recordField = context.Record.GetRecordField(result);
            if (recordField != null)
            {
              string str = recordField.ValuesAsString(false);
              nameValueCollection.Add(alias, str);
            }
            else
              logger.LogWarning("Workflow {WorkflowName}: The field mapping with alias '{FieldMappingAlias}' did not match any record fields. This is probably caused by the record field being marked as sensitive and the workflow has been set not to include sensitive data.", (object) workflowName, (object) mapping.Alias);
          }
          else
            logger.LogWarning("Workflow {WorkflowName}: The field mapping with alias '{FieldMappingAlias}' did not contain a valid field ID.", (object) workflowName, (object) mapping.Alias);
        }
      }
      return nameValueCollection;
    }
  }
}
