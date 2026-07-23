
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.Workflow.ValidateWorkflowSettingsController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;

using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Providers;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form.Workflow
{
    public class ValidateWorkflowSettingsController : FormWorkflowControllerBase
    {
        public ValidateWorkflowSettingsController(WorkflowCollection workflowCollection)
          : base(workflowCollection)
        {
        }

        [HttpPost("{id:guid}/validate-settings")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public IActionResult ValidateSettings(
          Guid id,
          ValidateWorkflowSettingsController.ValidateWorkflowSettingsModel model)
        {
            Umbraco.Forms.Core.WorkflowType workflow1 = this.WorkflowCollection[id];
            Dictionary<string, SettingAttribute> source = workflow1.Settings();
            Umbraco.Forms.Core.Models.Workflow workflow2 = new Umbraco.Forms.Core.Models.Workflow();
            Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            List<Exception> exceptionList = new List<Exception>();
            foreach (KeyValuePair<string, string> setting1 in (IEnumerable<KeyValuePair<string, string>>)model.Settings)
            {
                KeyValuePair<string, string> setting = setting1;
                KeyValuePair<string, SettingAttribute> keyValuePair = source.SingleOrDefault<KeyValuePair<string, SettingAttribute>>(x => string.Equals(x.Key, setting.Key, StringComparison.OrdinalIgnoreCase));
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
            return exceptionList.Any<Exception>() ? this.BadRequest(FormsManagementApiControllerBase.BuildSettingsValidationProblemDetails(exceptionList)) : this.Ok();
        }

        public class ValidateWorkflowSettingsModel
        {
            public string Name { get; set; } = string.Empty;

            public IDictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
        }
    }
}
