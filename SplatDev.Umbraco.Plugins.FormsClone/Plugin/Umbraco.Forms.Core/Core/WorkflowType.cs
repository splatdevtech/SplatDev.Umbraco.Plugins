
// Type: Umbraco.Forms.Core.WorkflowType
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections;
using System.Reflection;

using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Interfaces;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core
{
    public abstract class WorkflowType : ProviderBase, IWorkflowType
    {
        public Workflow? Workflow { get; private set; }

        internal async Task<WorkflowExecutionStatus> ExecuteWorkflowAsync(
          WorkflowExecutionContext context,
          Workflow workflow)
        {
            this.Workflow = workflow;
            return await this.ExecuteAsync(context).ConfigureAwait(false);
        }

        public abstract Task<WorkflowExecutionStatus> ExecuteAsync(
          WorkflowExecutionContext context);

        public abstract List<Exception> ValidateSettings();

        public void LoadSettings(Workflow workflow) => this.ApplySettings(workflow, null, null, null, null, null);

        public void LoadSettings(
          Workflow workflow,
          IPlaceholderParsingService placeholderParsingService,
          Record? record = null,
          Form? form = null,
          Hashtable? pageElements = null,
          IDictionary<string, string?>? additionalData = null)
        {
            this.ApplySettings(workflow, placeholderParsingService, record, form, pageElements, additionalData);
        }

        private void ApplySettings(
          Workflow workflow,
          IPlaceholderParsingService? placeholderParsingService,
          Record? record,
          Form? form,
          Hashtable? pageElements,
          IDictionary<string, string?>? additionalData)
        {
            this.Workflow = workflow;
            Dictionary<string, SettingAttribute> typeSettings = this.Settings();
            IDictionary<string, string> dictionary = placeholderParsingService == null ? workflow.Settings : workflow.Settings.ParseSettingsPlaceholders(placeholderParsingService, typeSettings, record, form, pageElements, additionalData);
            Type type = this.GetType();
            foreach (KeyValuePair<string, SettingAttribute> keyValuePair in typeSettings)
            {
                string empty;
                if (!dictionary.TryGetValue(keyValuePair.Key, out empty) || empty == null)
                    empty = string.Empty;
                type.InvokeMember(keyValuePair.Key, BindingFlags.SetProperty, null, this, new object[1]
                {
           empty
                });
            }
        }

        public virtual Dictionary<string, SettingAttribute> Settings()
        {
            Dictionary<string, SettingAttribute> dictionary = new Dictionary<string, SettingAttribute>(StringComparer.OrdinalIgnoreCase);
            foreach (PropertyInfo property in this.GetType().GetProperties())
            {
                SettingAttribute settingAttribute = property.GetCustomAttributes<SettingAttribute>(true).FirstOrDefault<SettingAttribute>();
                if (settingAttribute != null)
                {
                    if (string.IsNullOrWhiteSpace(settingAttribute.Alias))
                        settingAttribute.Alias = property.Name;
                    dictionary.Add(property.Name, settingAttribute);
                }
            }
            return dictionary;
        }
    }
}
