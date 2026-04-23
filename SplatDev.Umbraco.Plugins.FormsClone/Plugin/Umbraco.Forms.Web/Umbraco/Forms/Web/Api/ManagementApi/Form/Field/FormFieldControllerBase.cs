
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.Field.FormFieldControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form.Field
{
  [ApiExplorerSettings(GroupName = "Form")]
  [Authorize(Policy = "SectionAccessForms")]
  [Route("/umbraco/forms/management/api/v1/form-field")]
  public abstract class FormFieldControllerBase : FormsManagementApiControllerBase
  {
    protected FormFieldControllerBase(IFieldTypeStorage fieldTypeStorage) => this.FieldTypeStorage = fieldTypeStorage;

    protected IFieldTypeStorage FieldTypeStorage { get; }
  }
}
