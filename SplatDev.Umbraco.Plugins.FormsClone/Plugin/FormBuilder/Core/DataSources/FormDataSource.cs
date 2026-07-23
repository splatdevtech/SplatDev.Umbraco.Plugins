using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Providers.Collections;

using System.Runtime.Serialization;

namespace FormBuilder.Core.DataSources
{
    [DataContract(Name = "formDataSource")]
    public class FormDataSource : ITypeWithEditorDetails, IType
    {
        private FormDataSourceType? _formDataSourceType;

        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "unique")]
        public Guid Unique => Id;

        [DataMember(Name = "entityType")]
        public static string EntityType => "datasource";

        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "created")]
        public DateTime Created { get; set; }

        [DataMember(Name = "createdBy")]
        public int? CreatedBy { get; set; }

        [DataMember(Name = "createdByName")]
        public string? CreatedByName { get; set; }

        [DataMember(Name = "updated")]
        public DateTime Updated { get; set; }

        [DataMember(Name = "updatedBy")]
        public int? UpdatedBy { get; set; }

        [DataMember(Name = "updatedByName")]
        public string? UpdatedByName { get; set; }

        [DataMember(Name = "settings")]
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        [DataMember(Name = "formDataSourceTypeId")]
        public Guid FormDataSourceTypeId { get; set; }

        [DataMember(Name = "valid")]
        public bool Valid { get; set; }

        public FormDataSourceType? GetFormDataSourceType(
          DataSourceTypeCollection dataSourceTypeCollection)
        {
            if (_formDataSourceType is not null || FormDataSourceTypeId == Guid.Empty)
                return _formDataSourceType;
            _formDataSourceType = dataSourceTypeCollection[FormDataSourceTypeId];
            return _formDataSourceType;
        }

        public void SetFormDataSourceType(FormDataSourceType value)
        {
            FormDataSourceTypeId = value.Id;
            _formDataSourceType = value;
        }
    }
}