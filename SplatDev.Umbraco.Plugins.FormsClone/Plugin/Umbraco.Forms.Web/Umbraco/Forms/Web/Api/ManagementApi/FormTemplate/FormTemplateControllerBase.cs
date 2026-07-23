
// Type: Umbraco.Forms.Web.Api.ManagementApi.FormTemplate.FormTemplateControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Forms.Data.Storage;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.FormTemplate
{
  [ApiExplorerSettings(GroupName = "Form Template")]
  [Authorize(Policy = "SectionAccessForms")]
  [Route("/umbraco/forms/management/api/v1/form-template")]
  public abstract class FormTemplateControllerBase : FormsManagementApiControllerBase
  {
    protected FormTemplateControllerBase(IFormTemplateStorage formTemplateStorage) => this.FormTemplateStorage = formTemplateStorage;

    protected IFormTemplateStorage FormTemplateStorage { get; }
  }
}
