
// Type: Umbraco.Forms.Web.Api.ManagementApi.Record.GetAuditTrailController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Record
{
    public class GetAuditTrailController : RecordControllerBase
    {
        private readonly IRecordAuditStorage _recordAuditStorage;
        private readonly IUserService _userService;

        public GetAuditTrailController(
          IFormService formService,
          IRecordStorage recordStorage,
          IFormsSecurity formsSecurity,
          IPlaceholderParsingService placeholderParsingService,
          IRecordAuditStorage recordAuditStorage,
          IUserService userService)
          : base(formService, recordStorage, formsSecurity, placeholderParsingService)
        {
            this._recordAuditStorage = recordAuditStorage;
            this._userService = userService;
        }

        [HttpGet("{recordId:guid}/audit-trail")]
        [ProducesResponseType(typeof(IEnumerable<RecordAuditEntry>), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetAuditTrail(Guid formId, Guid recordId)
        {
            Umbraco.Forms.Core.Models.Form form = this.FormService.Get(formId);
            if (form == null)
                return this.NotFound();
            Umbraco.Forms.Core.Persistence.Dtos.Record recordByUniqueId = this.RecordStorage.GetRecordByUniqueId(recordId, form);
            if (recordByUniqueId == null)
                return this.NotFound();
            List<RecordAudit> recordAuditTrail = this._recordAuditStorage.GetRecordAuditTrail(recordByUniqueId.Id);
            IList<IUser> backOfficeUsers = this.GetBackOfficeRecordEditors(recordAuditTrail);
            return this.Ok(recordAuditTrail.Select<RecordAudit, RecordAuditEntry>(x => new RecordAuditEntry()
            {
                Id = x.Id,
                UpdatedBy = GetAuditTrailController.GetBackOfficeUserName(x.UpdatedBy, backOfficeUsers),
                UpdatedOn = x.UpdatedOn
            }).ToList<RecordAuditEntry>());
        }

        private IList<IUser> GetBackOfficeRecordEditors(List<RecordAudit> recordAuditTrail) => this._userService.GetUsersById(GetAuditTrailController.GetUserIdsForRecordEditors(recordAuditTrail)).ToList<IUser>();

        private static int[] GetUserIdsForRecordEditors(List<RecordAudit> recordAuditTrail) => recordAuditTrail.Where<RecordAudit>(x => x.UpdatedBy.HasValue).Select<RecordAudit, int>(x => x.UpdatedBy.Value).Distinct<int>().ToArray<int>();

        private static string GetBackOfficeUserName(int? updatedBy, IList<IUser> backOfficeUsers)
        {
            if (!updatedBy.HasValue)
                return string.Empty;
            IUser user = backOfficeUsers.SingleOrDefault<IUser>(x => x.Id == updatedBy.Value);
            return user == null ? string.Empty : user.Name ?? string.Empty;
        }
    }
}
