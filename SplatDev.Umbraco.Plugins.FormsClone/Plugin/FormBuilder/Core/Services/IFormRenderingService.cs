using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FormBuilder.Core.Services
{
    public interface IFormRenderingService
    {
        Task<FormViewModel> GetFormModelAsync(
          HttpContext httpContext,
          Guid formId,
          Guid? recordId = null,
          string theme = "",
          IDictionary<string, string?>? additionalData = null);

        /// <summary>
        /// Populates an item in the HttpContext.Items collection with elements from the current page, used in placeholder
        /// parsing when rendering the form.
        /// </summary>
        void PopulatePageElements(HttpContext httpContext);

        Form? GetForm(Guid formId);

        Record? GetRecord(Guid recordId, Form form);

        void PrePopulateForm(Form form, HttpContext httpContext, FormViewModel model, Record? record = null);

        void StoreFormModel(ITempDataDictionary tempData, FormViewModel model);

        void ClearFormModel(ITempDataDictionary tempData);

        Dictionary<string, object[]> GetFormState(
          Form form,
          FormViewModel model,
          HttpContext context);

        Dictionary<string, object[]> RetrieveFormState(FormViewModel model);

        void ResumeFormState(
          FormViewModel model,
          Dictionary<string, object[]> state,
          bool editSubmission = false);

        void StoreFormState(Dictionary<string, object[]> state, FormViewModel model);

        void ClearFormState(FormViewModel model);
    }
}