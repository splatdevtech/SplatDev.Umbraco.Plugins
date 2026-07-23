using FormBuilder.Core.Enums;

using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using Umbraco.Extensions;

namespace FormBuilder.Core.Persistence.Fields
{
    [TableName("FormBuilderRecords")]
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExplicitColumns]
    public class Record
    {
        public Record()
        {
            RecordFields = [];
            State = FormState.Opened;
            IP = string.Empty;
            MemberKey = null;
            UniqueId = Guid.NewGuid();
        }

        [Column(Name = "Id")]
        [PrimaryKeyColumn(IdentitySeed = 0)]
        public int Id { get; set; }

        [Column(Name = "Form")]
        public Guid Form { get; set; }

        [Column(Name = "Created")]
        public DateTime Created { get; set; }

        [Column(Name = "Updated")]
        public DateTime Updated { get; set; }

        [Ignore]
        public FormState State
        {
            get => Enum.Parse<FormState>(StateAsString);
            set => StateAsString = Enum.GetName(value)!;
        }

        [Column(Name = "CurrentPage")]
        [NullSetting]
        public Guid CurrentPage { get; set; }

        [Column(Name = "UmbracoPageId")]
        [NullSetting]
        public int UmbracoPageId { get; set; }

        [Column(Name = "IP")]
        [NullSetting]
        public string IP { get; set; } = string.Empty;

        [Column(Name = "MemberKey")]
        [NullSetting]
        public string? MemberKey { get; set; }

        [Column(Name = "UniqueId")]
        public Guid UniqueId { get; set; }

        [Column(Name = "State")]
        [NullSetting]
        [Length(50)]
        public string StateAsString { get; set; } = string.Empty;

        [Column(Name = "RecordData")]
        [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
        public string RecordData { get; set; } = string.Empty;

        [Column(Name = "Culture")]
        [NullSetting]
        [Length(84)]
        public string Culture { get; set; } = string.Empty;

        [Column(Name = "AdditionalData")]
        [NullSetting]
        [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
        public string? AdditionalData { get; set; }

        [Ignore]
        public Dictionary<Guid, RecordField> RecordFields { get; set; } = [];

        public string GenerateRecordDataAsJson()
        {
            string str = string.Empty;
            foreach (KeyValuePair<Guid, RecordField> recordField in RecordFields)
                str = str + "'" + recordField.Value.FieldId.ToString() + "':'" + recordField.Value.ValuesAsString() + "',";
            return "{" + str.TrimEnd(',') + "}";
        }

        public RecordField? GetRecordField(string name) => RecordFields.FirstOrDefault(m => m.Value.Field!.Caption.ToLower().Equals(name, StringComparison.InvariantCultureIgnoreCase)).Value;

        public RecordField? GetRecordFieldByAlias(string alias) => RecordFields.FirstOrDefault(m => m.Value?.Field?.Alias == alias).Value;

        public List<RecordField> GetRecordFieldsByType(Guid fieldTypeId)
        {
            List<RecordField> recordFieldsByType =
            [
                .. RecordFields.Where(x =>
                {
                    RecordField recordField = x.Value;
                    if (recordField is null)
                        return false;
                    Guid? fieldTypeId1 = recordField.Field?.FieldTypeId;
                    Guid guid = fieldTypeId;
                    return fieldTypeId1.HasValue && fieldTypeId1.GetValueOrDefault() == guid;
                }).Select(x => x.Value),
            ];
            return recordFieldsByType;
        }

        public RecordField? GetRecordField(Guid id)
        {
            if (RecordFields is null)
                return null;
            KeyValuePair<Guid, RecordField> keyValuePair = RecordFields.FirstOrDefault(x => x.Value.FieldId == id);
            if (keyValuePair.Value is null)
                return null;
            keyValuePair = RecordFields.FirstOrDefault(x => x.Value.FieldId == id);
            return keyValuePair.Value;
        }

        public IEnumerable<T> GetValues<T>(string alias)
        {
            if (RecordFields is null)
                return [];
            RecordField? recordField = RecordFields.Values.FirstOrDefault(x => x.Alias == alias);
            return recordField is null ? [] : recordField.Values.Select(x => x.TryConvertTo<T>()).Where(x => x.Success).Select(x => x.Result!);
        }

        public T? GetValue<T>(string alias) => GetValues<T>(alias).FirstOrDefault();
    }
}