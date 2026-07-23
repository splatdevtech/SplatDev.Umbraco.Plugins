
// Type: Umbraco.Forms.Core.Providers.PreValues.DataSource
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Providers.PreValues
{
  public class DataSource : FieldPreValueSourceType
  {
    private readonly IDataSourceService _dataSourceService;
    private readonly DataSourceTypeCollection _dataSourceTypeCollection;
    private readonly List<PreValue> _prevalues;

    public DataSource(
      IDataSourceService dataSourceService,
      DataSourceTypeCollection dataSourceTypeCollection)
    {
      this._dataSourceService = dataSourceService;
      this._dataSourceTypeCollection = dataSourceTypeCollection;
      this.Id = new Guid("CC9F9B2A-A746-11DE-9E17-681B56D89593");
      this.Name = nameof (DataSource);
      this.Alias = "dataSource";
      this.Description = "Stores prevalues in the prevalues table";
      this.Icon = "icon-box";
      this._prevalues = new List<PreValue>();
    }

    public override List<Exception> ValidateSettings() => new List<Exception>();

    public override Task<List<PreValue>> GetPreValuesAsync(Field? field, Form? form)
    {
      this._prevalues.Clear();
      if (field == null || form?.DataSource == null)
        return Task.FromResult<List<PreValue>>(this._prevalues);
      if (!string.IsNullOrEmpty(field.DataSourceFieldKey))
      {
        FormDataSource datasource = this._dataSourceService.Get(form.DataSource.Id);
        FormDataSourceType formDataSourceType = datasource?.GetFormDataSourceType(this._dataSourceTypeCollection);
        if (datasource != null && formDataSourceType != null)
        {
          formDataSourceType.LoadSettings(datasource);
          int num = 0;
          foreach (KeyValuePair<object, string> preValue in formDataSourceType.GetPreValues(field, form))
          {
            this._prevalues.Add(new PreValue()
            {
              Id = preValue.Key.ToString(),
              Value = preValue.Value,
              SortOrder = num
            });
            ++num;
          }
        }
      }
      return Task.FromResult<List<PreValue>>(this._prevalues);
    }
  }
}
