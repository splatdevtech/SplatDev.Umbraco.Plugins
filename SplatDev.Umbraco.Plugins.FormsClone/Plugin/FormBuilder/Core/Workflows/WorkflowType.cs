using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Extensions;
using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Providers;
using FormBuilder.Core.Services.Interfaces;

using System.Collections;
using System.Reflection;

namespace FormBuilder.Core.Workflows
{
    public abstract class WorkflowType : ProviderBase, IWorkflowType
    {
        public Workflow? Workflow { get; private set; }

        internal async Task<WorkflowExecutionStatus> ExecuteWorkflowAsync(
          WorkflowExecutionContext context,
          Workflow workflow)
        {
            Workflow = workflow;
            return await ExecuteAsync(context).ConfigureAwait(false);
        }

        public abstract Task<WorkflowExecutionStatus> ExecuteAsync(
          WorkflowExecutionContext context);

        public abstract List<Exception> ValidateSettings();

        public void LoadSettings(Workflow workflow) => ApplySettings(workflow, null, null, null, null, null);

        public void LoadSettings(
          Workflow workflow,
          IPlaceholderParsingService placeholderParsingService,
          Record? record = null,
          Form? form = null,
          Hashtable? pageElements = null,
          IDictionary<string, string?>? additionalData = null)
        {
            ApplySettings(workflow, placeholderParsingService, record, form, pageElements, additionalData);
        }

        private void ApplySettings(
          Workflow workflow,
          IPlaceholderParsingService? placeholderParsingService,
          Record? record,
          Form? form,
          Hashtable? pageElements,
          IDictionary<string, string?>? additionalData)
        {
            Workflow = workflow;
            Dictionary<string, SettingAttribute> typeSettings = Settings();
            IDictionary<string, string> dictionary = placeholderParsingService is null ? workflow.Settings : workflow.Settings.ParseSettingsPlaceholders(placeholderParsingService, typeSettings, record, form, pageElements, additionalData);
            Type type = GetType();
            foreach (KeyValuePair<string, SettingAttribute> keyValuePair in typeSettings)
            {
                if (!dictionary.TryGetValue(keyValuePair.Key, out string? empty) || empty is null)
                    empty = string.Empty;
                type.InvokeMember(keyValuePair.Key, BindingFlags.SetProperty, null, this,
                [
           empty
                ]);
            }
        }

        public virtual Dictionary<string, SettingAttribute> Settings()
        {
            Dictionary<string, SettingAttribute> dictionary = [];
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                SettingAttribute? settingAttribute = property.GetCustomAttributes<SettingAttribute>(true).FirstOrDefault();
                if (settingAttribute is not null)
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