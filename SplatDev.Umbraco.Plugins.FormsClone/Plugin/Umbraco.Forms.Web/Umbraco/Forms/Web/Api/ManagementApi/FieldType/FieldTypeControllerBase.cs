
// Type: Umbraco.Forms.Web.Api.ManagementApi.FieldType.FieldTypeControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.FieldType
{
  [ApiExplorerSettings(GroupName = "Field Type")]
  [Authorize(Policy = "SectionAccessForms")]
  [Route("/umbraco/forms/management/api/v1/field-type")]
  public abstract class FieldTypeControllerBase : FormsManagementApiControllerBase
  {
    protected FieldTypeControllerBase(IFieldTypeStorage fieldTypeStorage) => this.FieldTypeStorage = fieldTypeStorage;

    protected IFieldTypeStorage FieldTypeStorage { get; }
  }
}
