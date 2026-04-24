
// Type: Umbraco.Forms.Web.ViewComponents.RenderFormViewComponent
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Web.Models;
using Umbraco.Forms.Web.Services;


#nullable enable
namespace Umbraco.Forms.Web.ViewComponents
{
  public class RenderFormViewComponent : RenderFormViewComponentBase
  {
    private readonly PackageOptionSettings _packageOptionSettings;
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private IDataProtector? _dataProtector;

    public RenderFormViewComponent(
      IFormRenderingService formRenderingService,
      IFormThemeResolver formThemeResolver,
      IOptions<PackageOptionSettings> packageOptionSettings,
      IDataProtectionProvider dataProtectionProvider)
      : base(formRenderingService, formThemeResolver)
    {
      this._packageOptionSettings = packageOptionSettings.Value;
      this._dataProtectionProvider = dataProtectionProvider;
    }

    private IDataProtector DataProtector => this._dataProtector ?? (this._dataProtector = this._dataProtectionProvider.CreateProtector("Umbraco.Forms.AdditionalData"));

    public async Task<IViewComponentResult> InvokeAsync(
      Guid formId,
      Guid? recordId = null,
      string theme = "",
      bool includeScripts = true,
      Guid? redirectToPageId = null,
      IDictionary<string, string?>? additionalData = null)
    {
      RenderFormViewComponent formViewComponent = this;
      FormViewModel formModelAsync = await formViewComponent.FormRenderingService.GetFormModelAsync(formViewComponent.HttpContext, formId, recordId, theme, additionalData);
      if (formModelAsync.FormId == Guid.Empty)
        return (IViewComponentResult) formViewComponent.Content(string.Empty);
      formModelAsync.RenderScriptFiles = includeScripts;
      string formRender = formViewComponent.FormThemeResolver.GetFormRender(formModelAsync);
      formModelAsync.SubmitHandled = formViewComponent.WasFormSubmitted(formId);
      formModelAsync.RedirectToPageId = redirectToPageId;
      IDictionary<string, string> dictionary = additionalData;
      if ((dictionary != null ? (dictionary.Count > 0 ? 1 : 0) : 0) != 0)
      {
        string plaintext = JsonSerializer.Serialize<IDictionary<string, string>>(additionalData);
        formModelAsync.AdditionalData = formViewComponent.DataProtector.Protect(plaintext);
      }
      return (IViewComponentResult) formViewComponent.View<FormViewModel>(formRender, formModelAsync);
    }

    private bool WasFormSubmitted(Guid formId)
    {
      if (this.TempData != null && this.TempData["UmbracoFormSubmittedFromCurrentPage"] != null)
        return (Guid) this.TempData["UmbracoFormSubmittedFromCurrentPage"] == formId;
      Guid result;
      return this._packageOptionSettings.AppendQueryStringOnRedirectAfterFormSubmission && Guid.TryParse((string) this.Request.Query["formSubmitted"], out result) && result == formId;
    }
  }
}
