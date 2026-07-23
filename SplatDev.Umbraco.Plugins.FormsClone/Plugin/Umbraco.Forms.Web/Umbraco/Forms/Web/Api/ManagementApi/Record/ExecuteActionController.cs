
// Type: Umbraco.Forms.Web.Api.ManagementApi.Record.ExecuteActionController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;

using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Record
{
    public class ExecuteActionController : RecordControllerBase
    {
        private readonly RecordSetActionCollection _recordSetActions;

        public ExecuteActionController(
          IFormService formService,
          IRecordStorage recordStorage,
          IFormsSecurity formsSecurity,
          IPlaceholderParsingService placeholderParsingService,
          RecordSetActionCollection recordSetActions)
          : base(formService, recordStorage, formsSecurity, placeholderParsingService)
        {
            this._recordSetActions = recordSetActions;
        }

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
            if (ExecuteActionController.IsDeleteRecordsetAction(actionId) && !actionController.FormsSecurity.CanCurrentUserDeleteEntries())
                return actionController.Forbid();
            Umbraco.Forms.Core.Models.Form form = actionController.FormService.Get(formId);
            if (form == null)
                return actionController.NotFound();
            List<Umbraco.Forms.Core.Persistence.Dtos.Record> records = actionController.RecordStorage.GetRecords(model.RecordKeys, form);
            int num = (int)await actionController._recordSetActions[actionId].ExecuteAsync(records, form);
            return actionController.Ok();
        }

        private static bool IsDeleteRecordsetAction(Guid actionId) => actionId == Guid.Parse("CB126B70-9011-11DF-A4EE-0800200C9A66");
    }
}
