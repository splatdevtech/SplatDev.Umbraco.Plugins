
// Type: Umbraco.Forms.Web.Services.IFormRenderingService
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Web.Models;


#nullable enable
namespace Umbraco.Forms.Web.Services
{
  public interface IFormRenderingService
  {
    Task<FormViewModel> GetFormModelAsync(
      HttpContext httpContext,
      Guid formId,
      Guid? recordId = null,
      string theme = "",
      IDictionary<string, string?>? additionalData = null);

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
