using FormBuilder.Core.DataSources;
using FormBuilder.Core.Fields;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving a scaffold for creating a form via the datasource wizard.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class GetScaffoldController(
      IDataSourceService dataSourceStorage,
      DataSourceTypeCollection dataSourceTypeCollection) : DataSourceWizardControllerBase
    {
        private readonly IDataSourceService _dataSourceService = dataSourceStorage;
        private readonly DataSourceTypeCollection _dataSourceTypeCollection = dataSourceTypeCollection;

        /// <summary>
        /// Management API controller for endpoint an empty scaffold for creating a form via the datasource wizard.
        /// </summary>
        [HttpGet("{id:guid}/scaffold")]
        [ProducesResponseType(typeof(DataSourceWizard), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public IActionResult GetScaffold(Guid id)
        {
            DataSourceWizard dataSourceWizard = new();
            FormDataSource? datasource = _dataSourceService.Get(id);
            if (datasource is null)
                return NotFound();
            FormDataSourceType? formDataSourceType = datasource.GetFormDataSourceType(_dataSourceTypeCollection);
            if (formDataSourceType is null)
                return NotFound();
            formDataSourceType.LoadSettings(datasource);
            dataSourceWizard.FormName = "Form From " + datasource.Name;
            dataSourceWizard.DataSourceGuid = id;
            List<DataSourceWizardFieldMapping> wizardFieldMappingList = [];
            foreach (KeyValuePair<object, FormDataSourceField> availableField in formDataSourceType.GetAvailableFields())
            {
                FormDataSourceField formDataSourceField = availableField.Value;
                DataSourceWizardFieldMapping wizardFieldMapping = new()
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
            dataSourceWizard.Mappings = wizardFieldMappingList;
            return Ok(dataSourceWizard);
        }
    }
}