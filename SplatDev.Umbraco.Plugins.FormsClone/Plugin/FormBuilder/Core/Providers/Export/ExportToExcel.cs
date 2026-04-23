using FormBuilder.Core.Searches.Interfaces;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace FormBuilder.Core.Providers.Export
{
    /// <summary>
    /// Provides a an export type for exporting to an Excel spreadsheet.
    /// </summary>
    public class ExportToExcel : BaseExportToExcel
    {
        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public ExportToExcel(
          IHostEnvironment hostEnvironment,
          IHttpContextAccessor httpContextAccessor,
          IFormRecordSearcher formRecordSearcher,
          IFormService formService,
          IFieldTypeStorage fieldTypeStorage,
          IPrevalueSourceService prevalueSourceService,
          IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService)
          : base(hostEnvironment, httpContextAccessor, formRecordSearcher, formService, fieldTypeStorage, prevalueSourceService, fieldPreValueSourceTypeService)
        {
            Description = "Exports all submitted values for the form to Excel, in a format useful for integration with other systems.";
            Name = "Excel File (submitted values)";
            Alias = "excelFileSubmittedValues";
            Id = new Guid("94ED105A-87B3-4e1f-97CB-9A320AEE2745");
        }

        /// <inheritdoc />
        protected override bool ReplacePrevalueCaptions => false;
    }
}