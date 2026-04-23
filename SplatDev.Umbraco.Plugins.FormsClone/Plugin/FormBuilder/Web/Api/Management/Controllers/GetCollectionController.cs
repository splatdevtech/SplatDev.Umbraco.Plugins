using FormBuilder.Core.Models;
using FormBuilder.Core.Searches;
using FormBuilder.Core.Searches.Interfaces;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving a collection of records (form submissions).
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the         /// </remarks>
    public class GetCollectionController(
      IFormService formService,
      IRecordStorage recordStorage,
      IFormsSecurity formsSecurity,
      IPlaceholderParsingService placeholderParsingService,
      IFormRecordSearcher formRecordSearcher) : GetRecordsControllerBase(formService, recordStorage, formsSecurity, placeholderParsingService, formRecordSearcher)
    {

        /// <summary>
        /// Management API endpoint for retrieving a collection of records (form submissions).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(EntrySearchResultCollection), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult GetCollection(Guid formId, [FromQuery] RecordFilter model)
        {
            Form? form = FormService.Get(formId);
            if (form is null)
                return NotFound();
            EntrySearchResultCollection results = FormRecordSearcher.QueryDataBase(formId, model);
            ApplyFieldSelectionSettingsToSchema(results.Schema, form);
            ApplyPlaceholderReplacementsToFieldNames(results);
            return Ok(results);
        }

        private static void ApplyFieldSelectionSettingsToSchema(
          IEnumerable<EntrySearchResultSchema> schema,
          Form form)
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
                    RecordFieldDisplay? recordFieldDisplay = form.SelectedDisplayFields.SingleOrDefault(x => x.IsSystem == isSystemField && x.Alias == item.Alias);
                    if (recordFieldDisplay is not null)
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
            if (results.Schema is null)
                return;
            foreach (EntrySearchResultSchema searchResultSchema in results.Schema)
            {
                if (!string.IsNullOrEmpty(searchResultSchema.Name))
                    searchResultSchema.Name = PlaceholderParsingService.ParsePlaceHolders(searchResultSchema.Name, false);
            }
        }
    }
}