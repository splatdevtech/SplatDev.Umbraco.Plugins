
// Type: Umbraco.Forms.Web.Api.ManagementApi.DataSource.Wizard.CreateFormFromDataSourceWizardController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;

using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.DataSource.Wizard
{
    public class CreateFormFromDataSourceWizardController : DataSourceWizardControllerBase
    {
        private readonly IFormService _formService;
        private readonly IFieldPreValueSourceService _fieldPreValueSourceService;

        public CreateFormFromDataSourceWizardController(
          IFormService formService,
          IFieldPreValueSourceService fieldPreValueSourceService)
        {
            this._fieldPreValueSourceService = fieldPreValueSourceService;
            this._formService = formService;
        }

        [HttpPost("create-form")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public void CreateForm(DataSourceWizard wizard)
        {
            Umbraco.Forms.Core.Models.Form form = new Umbraco.Forms.Core.Models.Form()
            {
                Name = wizard.FormName,
                SubmitLabel = "Submit",
                MessageOnSubmit = "Thank you"
            };
            Page page = new Page() { Caption = "Page" };
            FieldSet fieldSet = new FieldSet()
            {
                Caption = "Fieldset"
            };
            FieldsetContainer fieldsetContainer = new FieldsetContainer()
            {
                Width = 12
            };
            this._formService.Insert(form);
            List<FormDataSourceMapping> dataSourceMappingList = new List<FormDataSourceMapping>();
            foreach (DataSourceWizardFieldMapping mapping in wizard.Mappings)
            {
                if (mapping.Include)
                {
                    FormDataSourceMapping dataSourceMapping = new FormDataSourceMapping()
                    {
                        DataType = mapping.DataType,
                        DataFieldKey = mapping.Key,
                        FormId = form.Id
                    };
                    Field field = new Field()
                    {
                        Id = Guid.NewGuid()
                    };
                    if (mapping.IsForeignKey)
                    {
                        dataSourceMapping.PrevalueKeyfield = mapping.PrevalueKeyField;
                        dataSourceMapping.PrevalueTable = mapping.PrevalueSource;
                        if (mapping.AvailablePrevalueValueFields != null)
                            dataSourceMapping.PrevalueValueField = mapping.PrevalueValueField;
                        field.PreValueSourceId = this._fieldPreValueSourceService.GetDefaultProvider().Id;
                    }
                    if (mapping.IsMandatory)
                        field.Mandatory = true;
                    field.Caption = CreateFormFromDataSourceWizardController.DePascalize(mapping.Name);
                    if (mapping.FieldTypeId != Guid.Empty)
                        field.FieldTypeId = mapping.FieldTypeId;
                    field.RegEx = string.Empty;
                    field.DataSourceFieldKey = mapping.Key;
                    field.ToolTip = mapping.Name;
                    if (!string.IsNullOrEmpty(mapping.DefaultValue))
                        dataSourceMapping.DefaultValue = mapping.DefaultValue;
                    else
                        fieldsetContainer.Fields.Add(field);
                    dataSourceMappingList.Add(dataSourceMapping);
                }
            }
            fieldSet.Containers.Add(fieldsetContainer);
            page.FieldSets.Add(fieldSet);
            form.Pages.Add(page);
            FormDataSourceDefinition sourceDefinition = new FormDataSourceDefinition()
            {
                Id = wizard.DataSourceGuid,
                Mappings = dataSourceMappingList
            };
            form.DataSource = sourceDefinition;
            this._formService.Update(form);
        }

        private static string DePascalize(string pascalString) => System.Text.RegularExpressions.Regex.Replace(System.Text.RegularExpressions.Regex.Replace(System.Text.RegularExpressions.Regex.Replace(System.Text.RegularExpressions.Regex.Replace(pascalString, "([^A-Z])([A-Z])", "$1 $2").Trim(), "([A-Z])([A-Z])([^A-Z])", "$1 $2$3").Trim(), "([A-Za-z])([0-9])", "$1 $2").Trim(), "([0-9])([A-Za-z])", "$1 $2").Trim();
    }
}
