using FormBuilder.Core.Export;
using FormBuilder.Core.Extensions;
using FormBuilder.Core.Models;
using FormBuilder.Core.Searches;
using FormBuilder.Core.Searches.Interfaces;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Web.Helpers;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

using OfficeOpenXml;

using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;

namespace FormBuilder.Core.Providers.Export
{
    /// <summary>
    /// Provides a an export type for exporting to an Excel spreadsheet.
    /// </summary>
    public abstract class BaseExportToExcel : ExportType
    {
        private readonly IFormRecordSearcher _formRecordSearcher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFormService _formService;
        private readonly IFieldTypeStorage _fieldTypeStorage;
        private readonly IPrevalueSourceService _prevalueSourceService;
        private readonly IFieldPrevalueSourceTypeService _fieldPreValueSourceTypeService;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        protected BaseExportToExcel(
          IHostEnvironment hostEnvironment,
          IHttpContextAccessor httpContextAccessor,
          IFormRecordSearcher formRecordSearcher,
          IFormService formService,
          IFieldTypeStorage fieldTypeStorage,
          IPrevalueSourceService prevalueSourceService,
          IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService)
          : base(hostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _formRecordSearcher = formRecordSearcher;
            _formService = formService;
            _fieldTypeStorage = fieldTypeStorage;
            _prevalueSourceService = prevalueSourceService;
            _fieldPreValueSourceTypeService = fieldPreValueSourceTypeService;
            FileExtension = "xlsx";
            Icon = "icon-forms-file-excel";
        }

        /// <summary>
        /// Gets a value indicating whether to replace prevalue caption or output the raw, submitted values.
        /// </summary>
        protected virtual bool ReplacePrevalueCaptions => false;

        /// <inheritdoc />
        public override async Task<string> ExportToFileAsync(
          Guid formId,
          RecordExportFilter filter,
          string filepath)
        {
            BaseExportToExcel baseExportToExcel = this;
            EntrySearchResultCollection? submissions = baseExportToExcel._formRecordSearcher.QueryDataBase(formId, filter);
            List<EntrySearchResultSchema>? schemaItems = [.. submissions.Schema];
            Dictionary<Guid, Dictionary<string, string?>?>? prevalueMaps = await baseExportToExcel.GetPrevalueMaps(formId);
            bool flag = CanAutoFitColumns();
            string? fileAsync;
            using (var excelPackage = new ExcelPackage())
            {
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("Submissions");
                List<string?> list1 = [.. submissions.Schema.Select(x => x.Name)];
                int Row1 = 1;
                for (int index = 0; index < list1.Count; ++index)
                    excelWorksheet.Cells[Row1, index + 1].Value = list1[index];
                List<EntrySearchResult> list2 = [.. submissions.Results];
                for (int index1 = 0; index1 < submissions.Results.Count(); ++index1)
                {
                    int Row2 = index1 + 2;
                    List<EntrySearchResult.FieldData> list3 = [.. list2[index1].Fields];
                    for (int index2 = 0; index2 < list3.Count; ++index2)
                    {
                        int Col = index2 + 1;
                        object? obj = list3[index2].Value;
                        string? str1 = obj?.ToString();
                        if (baseExportToExcel.ReplacePrevalueCaptions && !string.IsNullOrWhiteSpace(str1) && Guid.TryParse(list3[index2].FieldId, out Guid result))
                            str1 = str1.ApplyPrevalueCaptions(result, prevalueMaps);
                        if (!string.IsNullOrWhiteSpace(str1) && str1.StartsWith('='))
                            str1 = "'" + str1;
                        excelWorksheet.Cells[Row2, Col].Value = str1;
                        if (flag)
                            SafeAutoFitColumns(excelWorksheet.Cells[Row2, Col]);
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
                        if (searchResultSchema is not null && searchResultSchema.View.Equals("file", StringComparison.CurrentCultureIgnoreCase))
                        {
                            string? str2 = baseExportToExcel._httpContextAccessor.HttpContext?.Request.Host.Value;
                            string[] strArray = (obj?.ToString() ?? string.Empty).Split(',');
                            for (int index3 = 0; index3 < strArray.Length; ++index3)
                            {
                                string? str3 = strArray[index3].TrimStart(' ');
                                strArray[index3] = string.IsNullOrEmpty(str2) || str3.StartsWith("http") ? str3 : str2 + str3;
                            }
                            string? str4 = string.Join(",", strArray);
                            excelWorksheet.Cells[Row2, Col].Value = str4;
                        }
                    }
                }
                if (flag)
                    SafeAutoFitAllColumns(excelWorksheet);
                ExcelRow excelRow = excelWorksheet.Row(1);
                excelRow.Style.Font.Bold = true;
                excelRow.Style.Font.Size = 12f;
                string? str = filepath;
                if (!filepath.Contains(Path.DirectorySeparatorChar))
                    str = baseExportToExcel.HostEnvironment.MapPathContentRoot(filepath);
                string? path = filepath[..filepath.LastIndexOf(Path.DirectorySeparatorChar)];
                if (!str.EndsWith("." + baseExportToExcel.FileExtension))
                {
                    str = str + "." + baseExportToExcel.FileExtension;
                }
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (System.IO.File.Exists(str))
                    System.IO.File.Delete(str);
                excelPackage.SaveAs(new FileInfo(str));
                fileAsync = str;
            }
            submissions = null;
            schemaItems = null;
            return fileAsync;
        }

        private async Task<Dictionary<Guid, Dictionary<string, string?>?>?> GetPrevalueMaps(
          Guid formId)
        {
            Form? form = _formService.Get(formId);
            return form is null ? [] : await form.GetPrevalueMaps(_fieldTypeStorage, _prevalueSourceService, _fieldPreValueSourceTypeService).ConfigureAwait(false);
        }

        private static bool CanAutoFitColumns() => OperatingSystem.IsWindows();

        private static void SafeAutoFitColumns(ExcelRange excelRange)
        {
            try
            {
                excelRange.AutoFitColumns();
            }
            finally { }
        }

        private static void SafeAutoFitAllColumns(ExcelWorksheet excelWorksheet)
        {
            try
            {
                SafeAutoFitColumns(excelWorksheet.Cells[excelWorksheet.Dimension.Address]);
            }
            finally { }
        }

        /// <inheritdoc />
        public override async Task<string> ExportRecordsAsync(
          Guid formId,
          RecordExportFilter filter)
        {
            string? filePath = "~/Views/Partials/Forms/Export/excel.cshtml";
            EntrySearchResultCollection model = _formRecordSearcher.QueryDataBase(formId, filter);
            return await ViewHelper.RenderPartialViewToString(_httpContextAccessor.GetRequiredHttpContext(), filePath, model).ConfigureAwait(false);
        }
    }
}