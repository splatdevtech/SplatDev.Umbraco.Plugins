using FormBuilder.Core.DataSources;
using FormBuilder.Core.Models;
using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;

namespace FormBuilder.Core.Providers.Prevalues
{
    /// <summary>
    /// Defines a     /// </summary>
    public class DataSource : FieldPrevalueSourceType
    {
        private readonly IDataSourceService _dataSourceService;
        private readonly DataSourceTypeCollection _dataSourceTypeCollection;
        private readonly List<Prevalue> _prevalues;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public DataSource(
          IDataSourceService dataSourceService,
          DataSourceTypeCollection dataSourceTypeCollection)
        {
            _dataSourceService = dataSourceService;
            _dataSourceTypeCollection = dataSourceTypeCollection;
            Id = new Guid("CC9F9B2A-A746-11DE-9E17-681B56D89593");
            Name = nameof(DataSource);
            Alias = "dataSource";
            Description = "Stores prevalues in the prevalues table";
            Icon = "icon-box";
            _prevalues = [];
        }

        /// <inheritdoc />
        public override List<Exception> ValidateSettings() => [];

        /// <inheritdoc />
        public override Task<List<Prevalue>> GetPreValuesAsync(Field? field, Form? form)
        {
            _prevalues.Clear();
            if (field == null || form?.DataSource == null)
                return Task.FromResult(_prevalues);
            if (!string.IsNullOrEmpty(field.DataSourceFieldKey))
            {
                FormDataSource? datasource = _dataSourceService.Get(form.DataSource.Id);
                FormDataSourceType? formDataSourceType = datasource?.GetFormDataSourceType(_dataSourceTypeCollection);
                if (datasource is not null && formDataSourceType is not null)
                {
                    formDataSourceType.LoadSettings(datasource);
                    int num = 0;
                    foreach (KeyValuePair<object, string> preValue in formDataSourceType.GetPreValues(field, form))
                    {
                        _prevalues.Add(new Prevalue()
                        {
                            Id = preValue.Key.ToString()!,
                            Value = preValue.Value,
                            SortOrder = num
                        });
                        ++num;
                    }
                }
            }
            return Task.FromResult(_prevalues);
        }
    }
}