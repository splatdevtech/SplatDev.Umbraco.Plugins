using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Core.Models.Membership;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving the audit trail of edits of a given record.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class GetAuditTrailController(
      IFormService formService,
      IRecordStorage recordStorage,
      IFormsSecurity formsSecurity,
      IPlaceholderParsingService placeholderParsingService,
      IRecordAuditStorage recordAuditStorage,
      IUserService userService) : RecordControllerBase(formService, recordStorage, formsSecurity, placeholderParsingService)
    {
        private readonly IRecordAuditStorage _recordAuditStorage = recordAuditStorage;
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Management API endpoint for retrieving the audit trail of edits of a given record.
        /// </summary>
        [HttpGet("{recordId:guid}/audit-trail")]
        [ProducesResponseType(typeof(IEnumerable<RecordAuditEntry>), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetAuditTrail(Guid formId, Guid recordId)
        {
            Form? form = FormService.Get(formId);
            if (form is null)
                return NotFound();
            Record? recordByUniqueId = RecordStorage.GetRecordByUniqueId(recordId, form);
            if (recordByUniqueId is null)
                return NotFound();
            List<RecordAudit> recordAuditTrail = _recordAuditStorage.GetRecordAuditTrail(recordByUniqueId.Id);
            IList<IUser> backOfficeUsers = GetBackOfficeRecordEditors(recordAuditTrail);
            return Ok(recordAuditTrail.Select(x => new RecordAuditEntry()
            {
                Id = x.Id,
                UpdatedBy = GetBackOfficeUserName(x.UpdatedBy, backOfficeUsers),
                UpdatedOn = x.UpdatedOn
            }).ToList());
        }

        private IList<IUser> GetBackOfficeRecordEditors(List<RecordAudit> recordAuditTrail) => [.. _userService.GetUsersById(GetUserIdsForRecordEditors(recordAuditTrail))];

        private static int[] GetUserIdsForRecordEditors(List<RecordAudit> recordAuditTrail) => [.. recordAuditTrail.Where(x => x.UpdatedBy.HasValue).Select(x => x.UpdatedBy!.Value).Distinct()];

        private static string GetBackOfficeUserName(int? updatedBy, IList<IUser> backOfficeUsers)
        {
            if (!updatedBy.HasValue)
                return string.Empty;
            IUser? user = backOfficeUsers.SingleOrDefault(x => x.Id == updatedBy.Value);
            return user is null ? string.Empty : user.Name ?? string.Empty;
        }
    }
}