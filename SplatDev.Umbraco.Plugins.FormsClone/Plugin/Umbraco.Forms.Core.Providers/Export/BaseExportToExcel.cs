
// Type: Umbraco.Forms.Core.Providers.Export.BaseExportToExcel
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

using OfficeOpenXml;

using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Searchers;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Helpers;


#nullable enable
namespace Umbraco.Forms.Core.Providers.Export
{
    public abstract class BaseExportToExcel : ExportType
    {
        private readonly IFormRecordSearcher _formRecordSearcher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFormService _formService;
        private readonly IFieldTypeStorage _fieldTypeStorage;
        private readonly IPrevalueSourceService _prevalueSourceService;
        private readonly IFieldPreValueSourceTypeService _fieldPreValueSourceTypeService;

        protected BaseExportToExcel(
          IHostEnvironment hostEnvironment,
          IHttpContextAccessor httpContextAccessor,
          IFormRecordSearcher formRecordSearcher,
          IFormService formService,
          IFieldTypeStorage fieldTypeStorage,
          IPrevalueSourceService prevalueSourceService,
          IFieldPreValueSourceTypeService fieldPreValueSourceTypeService)
          : base(hostEnvironment)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._formRecordSearcher = formRecordSearcher;
            this._formService = formService;
            this._fieldTypeStorage = fieldTypeStorage;
            this._prevalueSourceService = prevalueSourceService;
            this._fieldPreValueSourceTypeService = fieldPreValueSourceTypeService;
            this.FileExtension = "xlsx";
            this.Icon = "icon-forms-file-excel";
        }

        protected virtual bool ReplacePrevalueCaptions => false;

        protected override async Task ExportToSteamAsync(
          Guid formId,
          RecordExportFilter filter,
          Stream stream)
        {
            EntrySearchResultCollection submissions = this._formRecordSearcher.QueryDataBase(formId, filter);
            List<EntrySearchResultSchema> schemaItems = submissions.Schema.ToList<EntrySearchResultSchema>();
            Dictionary<Guid, Dictionary<string, string>> prevalueMaps = await this.GetPrevalueMaps(formId);
            bool flag = BaseExportToExcel.CanAutoFitColumns();
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("Submissions");
                List<string> list1 = submissions.Schema.Select<EntrySearchResultSchema, string>(x => x.Name).ToList<string>();
                int Row1 = 1;
                for (int index = 0; index < list1.Count<string>(); ++index)
                    excelWorksheet.Cells[Row1, index + 1].Value = list1[index];
                List<EntrySearchResult> list2 = submissions.Results.ToList<EntrySearchResult>();
                for (int index1 = 0; index1 < submissions.Results.Count<EntrySearchResult>(); ++index1)
                {
                    int Row2 = index1 + 2;
                    List<EntrySearchResult.FieldData> list3 = list2[index1].Fields.ToList<EntrySearchResult.FieldData>();
                    for (int index2 = 0; index2 < list3.Count<EntrySearchResult.FieldData>(); ++index2)
                    {
                        int Col = index2 + 1;
                        object obj = list3[index2].Value;
                        string str1 = obj?.ToString();
                        Guid result;
                        if (this.ReplacePrevalueCaptions && !string.IsNullOrWhiteSpace(str1) && Guid.TryParse(list3[index2].FieldId, out result))
                            str1 = str1.ApplyPrevalueCaptions(result, prevalueMaps);
                        if (!string.IsNullOrWhiteSpace(str1) && str1.StartsWith("="))
                            str1 = "'" + str1;
                        excelWorksheet.Cells[Row2, Col].Value = str1;
                        if (flag)
                            BaseExportToExcel.SafeAutoFitColumns(excelWorksheet.Cells[Row2, Col]);
                        switch (obj)
                        {
                            case DateTime _:
                                excelWorksheet.Cells[Row2, Col].Style.Numberformat.Format = "dd/mm/yyyy hh:mm";
                                break;
                            case IMember member:
                                excelWorksheet.Cells[Row2, Col].Value = member.Email;
                                break;
                        }
                        EntrySearchResultSchema searchResultSchema = schemaItems[index2];
                        if (searchResultSchema != null && searchResultSchema.View.ToLower() == "file")
                        {
                            string str2 = this._httpContextAccessor.HttpContext?.Request.Host.Value;
                            string[] strArray = (obj?.ToString() ?? string.Empty).Split(',');
                            for (int index3 = 0; index3 < strArray.Length; ++index3)
                            {
                                string str3 = strArray[index3].TrimStart(' ');
                                strArray[index3] = string.IsNullOrEmpty(str2) || str3.StartsWith("http") ? str3 : str2 + str3;
                            }
                            string str4 = string.Join(",", strArray);
                            excelWorksheet.Cells[Row2, Col].Value = str4;
                        }
                    }
                }
                if (flag)
                    BaseExportToExcel.SafeAutoFitAllColumns(excelWorksheet);
                ExcelRow excelRow = excelWorksheet.Row(1);
                excelRow.Style.Font.Bold = true;
                excelRow.Style.Font.Size = 12f;
                excelPackage.SaveAs(stream);
            }
            submissions = null;
            schemaItems = null;
        }

        private async Task<Dictionary<Guid, Dictionary<string, string?>>> GetPrevalueMaps(
          Guid formId)
        {
            Form form = this._formService.Get(formId);
            return form == null ? new Dictionary<Guid, Dictionary<string, string>>() : await form.GetPrevalueMaps(this._fieldTypeStorage, this._prevalueSourceService, this._fieldPreValueSourceTypeService).ConfigureAwait(false);
        }

        private static bool CanAutoFitColumns() => OperatingSystem.IsWindows();

        private static void SafeAutoFitColumns(ExcelRange excelRange)
        {
            try
            {
                excelRange.AutoFitColumns();
            }
            catch
            {
            }
        }

        private static void SafeAutoFitAllColumns(ExcelWorksheet excelWorksheet)
        {
            try
            {
                BaseExportToExcel.SafeAutoFitColumns(excelWorksheet.Cells[excelWorksheet.Dimension.Address]);
            }
            catch
            {
            }
        }

        public override async Task<string> ExportRecordsAsync(
          Guid formId,
          RecordExportFilter filter)
        {
            string filePath = "~/Views/Partials/Forms/Export/excel.cshtml";
            EntrySearchResultCollection model = this._formRecordSearcher.QueryDataBase(formId, filter);
            return await ViewHelper.RenderPartialViewToString(HttpContextAccessorExtensions.GetRequiredHttpContext(this._httpContextAccessor), filePath, model).ConfigureAwait(false);
        }
    }
}
