using FormBuilder.Core.Searches.Interfaces;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace FormBuilder.Core.Providers.Export
{
    /// <summary>
    /// Provides a an export type for exporting to an Excel spreadsheet (that replaces raw submitted values with prevalue captions when defined).
    /// </summary>
    public class ExportToExcelWithDisplayValues : BaseExportToExcel
    {
        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public ExportToExcelWithDisplayValues(
          IHostEnvironment hostEnvironment,
          IHttpContextAccessor httpContextAccessor,
          IFormRecordSearcher formRecordSearcher,
          IFormService formService,
          IFieldTypeStorage fieldTypeStorage,
          IPrevalueSourceService prevalueSourceService,
          IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService)
          : base(hostEnvironment, httpContextAccessor, formRecordSearcher, formService, fieldTypeStorage, prevalueSourceService, fieldPreValueSourceTypeService)
        {
            Description = "Exports all values for the form to Excel, in a format useful for reviewing the data or creating a report. Captions are used for prevalue data where available.";
            Name = "Excel File (display values)";
            Alias = "excelFileDisplayValues";
            Id = new Guid("688711A2-DC6F-4B51-B8D2-0BB177BB0499");
        }

        /// <inheritdoc />
        protected override bool ReplacePrevalueCaptions => true;
    }
}