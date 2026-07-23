using FormBuilder.Core.Attributes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Workflows;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for validating the settings for a field.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class ValidateWorkflowSettingsController(WorkflowCollection workflowCollection) : FormWorkflowControllerBase(workflowCollection)
    {
        /// <summary>
        /// Management API endpoint for validating the settings for a field.
        /// </summary>
        [HttpPost("{id:guid}/validate-settings")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public IActionResult ValidateSettings(
          Guid id,
          ValidateWorkflowSettingsModel model)
        {
            WorkflowType workflow1 = WorkflowCollection[id];
            Dictionary<string, SettingAttribute> source = workflow1.Settings();
            Workflow workflow2 = new();
            Dictionary<string, string> dictionary = new(StringComparer.OrdinalIgnoreCase);
            List<Exception> exceptionList = [];
            foreach (KeyValuePair<string, string> setting1 in (IEnumerable<KeyValuePair<string, string>>)model.Settings)
            {
                KeyValuePair<string, string> setting = setting1;
                KeyValuePair<string, SettingAttribute> keyValuePair = source.SingleOrDefault(x => string.Equals(x.Key, setting.Key, StringComparison.OrdinalIgnoreCase));
                if (string.IsNullOrEmpty(keyValuePair.Key))
                    exceptionList.Add(new KeyNotFoundException("The alias '" + setting.Key + "' was not present in the workflow type settings."));
                else
                    dictionary.Add(keyValuePair.Key, setting.Value);
            }
            workflow2.Settings = dictionary;
            workflow1.LoadSettings(workflow2);
            exceptionList.AddRange(workflow1.ValidateSettings());
            if (string.IsNullOrWhiteSpace(model.Name))
                exceptionList.Add(new Exception("A name for the workflow must be provided."));
            return exceptionList.Count != 0 ? BadRequest(FormsManagementApiControllerBase.BuildSettingsValidationProblemDetails(exceptionList)) : Ok();
        }

        /// <summary>Model POSTed to validate workflow settings.</summary>
        public class ValidateWorkflowSettingsModel
        {
            /// <summary>Gets or sets the workflow's name.</summary>
            public string Name { get; set; } = string.Empty;

            /// <summary>Gets or sets the field's settings.</summary>
            public IDictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
        }
    }
}