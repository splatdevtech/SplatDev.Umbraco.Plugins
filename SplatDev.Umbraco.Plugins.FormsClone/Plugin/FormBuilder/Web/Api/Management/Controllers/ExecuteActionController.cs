using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for executing an action on a collection of records.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class ExecuteActionController(
      IFormService formService,
      IRecordStorage recordStorage,
      IFormsSecurity formsSecurity,
      IPlaceholderParsingService placeholderParsingService,
      RecordSetActionCollection recordSetActions) : RecordControllerBase(formService, recordStorage, formsSecurity, placeholderParsingService)
    {
        private readonly RecordSetActionCollection _recordSetActions = recordSetActions;

        /// <summary>
        /// Management API endpoint for executing an action on a collection of records.
        /// </summary>
        [HttpPost("actions/{actionId:guid}/execute")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Execute(
          Guid formId,
          Guid actionId,
          RecordActionExecution model)
        {
            ExecuteActionController actionController = this;
            if (IsDeleteRecordsetAction(actionId) && !actionController.FormsSecurity.CanCurrentUserDeleteEntries())
                return actionController.Forbid();
            Form? form = actionController.FormService.Get(formId);
            if (form is null)
                return actionController.NotFound();
            List<Record> records = actionController.RecordStorage.GetRecords(model.RecordKeys, form);
            _ = (int)await actionController._recordSetActions[actionId].ExecuteAsync(records, form);
            return actionController.Ok();
        }

        private static bool IsDeleteRecordsetAction(Guid actionId) => actionId == Guid.Parse("CB126B70-9011-11DF-A4EE-0800200C9A66");
    }
}