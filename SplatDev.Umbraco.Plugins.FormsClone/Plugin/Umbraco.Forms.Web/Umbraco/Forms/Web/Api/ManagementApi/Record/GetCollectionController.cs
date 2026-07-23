
// Type: Umbraco.Forms.Web.Api.ManagementApi.Record.GetCollectionController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;

using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Searchers;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Record
{
    public class GetCollectionController : GetRecordsControllerBase
    {
        public GetCollectionController(
          IFormService formService,
          IRecordStorage recordStorage,
          IFormsSecurity formsSecurity,
          IPlaceholderParsingService placeholderParsingService,
          IFormRecordSearcher formRecordSearcher)
          : base(formService, recordStorage, formsSecurity, placeholderParsingService, formRecordSearcher)
        {
        }

        [HttpGet]
        [ProducesResponseType(typeof(EntrySearchResultCollection), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult GetCollection(Guid formId, [FromQuery] RecordFilter model)
        {
            Umbraco.Forms.Core.Models.Form form = this.FormService.Get(formId);
            if (form == null)
                return this.NotFound();
            EntrySearchResultCollection results = this.FormRecordSearcher.QueryDataBase(formId, model);
            this.ApplyFieldSelectionSettingsToSchema(results.Schema, form);
            this.ApplyPlaceholderReplacementsToFieldNames(results);
            return this.Ok(results);
        }

        private void ApplyFieldSelectionSettingsToSchema(
          IEnumerable<EntrySearchResultSchema> schema,
          Umbraco.Forms.Core.Models.Form form)
        {
            if (form.DisplayDefaultFields)
            {
                int num = 1;
                foreach (EntrySearchResultSchema searchResultSchema in schema)
                {
                    if (IsSystemField(searchResultSchema))
                        break;
                    searchResultSchema.ShowOnListingScreen = num <= 3;
                    ++num;
                }
            }
            else
            {
                foreach (EntrySearchResultSchema searchResultSchema in schema)
                {
                    EntrySearchResultSchema item = searchResultSchema;
                    bool isSystemField = IsSystemField(item);
                    RecordFieldDisplay recordFieldDisplay = form.SelectedDisplayFields.SingleOrDefault<RecordFieldDisplay>(x => x.IsSystem == isSystemField && x.Alias == item.Alias);
                    if (recordFieldDisplay != null)
                    {
                        item.ShowOnListingScreen = true;
                        item.Name = recordFieldDisplay.Caption;
                    }
                    else
                        item.ShowOnListingScreen = false;
                }
            }

            static bool IsSystemField(EntrySearchResultSchema item) => !Guid.TryParse(item.Id, out Guid _);
        }

        private void ApplyPlaceholderReplacementsToFieldNames(EntrySearchResultCollection results)
        {
            if (results.Schema == null)
                return;
            foreach (EntrySearchResultSchema searchResultSchema in results.Schema)
            {
                if (!string.IsNullOrEmpty(searchResultSchema.Name))
                    searchResultSchema.Name = this.PlaceholderParsingService.ParsePlaceHolders(searchResultSchema.Name, false);
            }
        }
    }
}
