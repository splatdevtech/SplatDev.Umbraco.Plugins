
// Type: Umbraco.Forms.Web.Api.ManagementApi.DataSource.Wizard.GetScaffoldController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.DataSource.Wizard
{
  public class GetScaffoldController : DataSourceWizardControllerBase
  {
    private readonly IDataSourceService _dataSourceService;
    private readonly DataSourceTypeCollection _dataSourceTypeCollection;

    public GetScaffoldController(
      IDataSourceService dataSourceStorage,
      DataSourceTypeCollection dataSourceTypeCollection)
    {
      this._dataSourceService = dataSourceStorage;
      this._dataSourceTypeCollection = dataSourceTypeCollection;
    }

    [HttpGet("{id:guid}/scaffold")]
    [ProducesResponseType(typeof (DataSourceWizard), 200)]
    [ProducesResponseType(typeof (ProblemDetails), 404)]
    [ProducesResponseType(typeof (ProblemDetails), 500)]
    public IActionResult GetScaffold(Guid id)
    {
      DataSourceWizard dataSourceWizard = new DataSourceWizard();
      FormDataSource datasource = this._dataSourceService.Get(id);
      if (datasource == null)
        return (IActionResult) this.NotFound();
      FormDataSourceType formDataSourceType = datasource.GetFormDataSourceType(this._dataSourceTypeCollection);
      if (formDataSourceType == null)
        return (IActionResult) this.NotFound();
      formDataSourceType.LoadSettings(datasource);
      dataSourceWizard.FormName = "Form From " + datasource.Name;
      dataSourceWizard.DataSourceGuid = id;
      List<DataSourceWizardFieldMapping> wizardFieldMappingList = new List<DataSourceWizardFieldMapping>();
      foreach (KeyValuePair<object, FormDataSourceField> availableField in formDataSourceType.GetAvailableFields())
      {
        FormDataSourceField formDataSourceField = availableField.Value;
        DataSourceWizardFieldMapping wizardFieldMapping = new DataSourceWizardFieldMapping()
        {
          Name = formDataSourceField.Name,
          Key = formDataSourceField.Key,
          IsForeignKey = formDataSourceField.IsForeignKey,
          DataType = formDataSourceField.FieldDataType,
          IsMandatory = formDataSourceField.IsMandatory,
          Include = !formDataSourceField.AutoIncrement && (formDataSourceField.IsMandatory || !formDataSourceField.AllowNulls),
          PrevalueKeyField = formDataSourceField.PreValueKeyField,
          AvailablePrevalueValueFields = formDataSourceField.AvailablePreValueValueFields,
          PrevalueSource = formDataSourceField.PreValueSource,
          IsPrimaryKey = formDataSourceField.IsPrimaryKey,
          AllowNulls = formDataSourceField.AllowNulls
        };
        wizardFieldMappingList.Add(wizardFieldMapping);
      }
      dataSourceWizard.Mappings = (IEnumerable<DataSourceWizardFieldMapping>) wizardFieldMappingList;
      return (IActionResult) this.Ok((object) dataSourceWizard);
    }
  }
}
