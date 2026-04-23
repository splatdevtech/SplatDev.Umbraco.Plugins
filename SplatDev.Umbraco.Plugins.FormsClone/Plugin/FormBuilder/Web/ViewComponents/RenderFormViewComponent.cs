using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using System.Text.Json;

namespace FormBuilder.Web.ViewComponents
{
    public class RenderFormViewComponent(
      IFormRenderingService formRenderingService,
      IFormThemeResolver formThemeResolver,
      IOptions<PackageOptionSettings> packageOptionSettings,
      IDataProtectionProvider dataProtectionProvider) : RenderFormViewComponentBase(formRenderingService, formThemeResolver)
    {
        private readonly PackageOptionSettings _packageOptionSettings = packageOptionSettings.Value;
        private readonly IDataProtectionProvider _dataProtectionProvider = dataProtectionProvider;
        private IDataProtector? _dataProtector;

        private IDataProtector DataProtector => _dataProtector ??= _dataProtectionProvider.CreateProtector("FormBuilderAdditionalData");

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
                return formViewComponent.Content(string.Empty);
            formModelAsync.RenderScriptFiles = includeScripts;
            string? formRender = formViewComponent.FormThemeResolver.GetFormRender(formModelAsync);
            formModelAsync.SubmitHandled = formViewComponent.WasFormSubmitted(formId);
            formModelAsync.RedirectToPageId = redirectToPageId;
            IDictionary<string, string?>? dictionary = additionalData;
            if ((dictionary is not null ? dictionary.Count > 0 ? 1 : 0 : 0) != 0)
            {
                string plaintext = JsonSerializer.Serialize(additionalData);
                formModelAsync.AdditionalData = formViewComponent.DataProtector.Protect(plaintext);
            }
            return formViewComponent.View(formRender, formModelAsync);
        }

        private bool WasFormSubmitted(Guid formId)
        {
            if (TempData is not null && TempData["FormBuilderSubmittedFromCurrentPage"] is not null)
                return (Guid)TempData["FormBuilderSubmittedFromCurrentPage"]! == formId;
            return _packageOptionSettings.AppendQueryStringOnRedirectAfterFormSubmission && Guid.TryParse((string)Request.Query["formSubmitted"]!, out Guid result) && result == formId;
        }
    }
}