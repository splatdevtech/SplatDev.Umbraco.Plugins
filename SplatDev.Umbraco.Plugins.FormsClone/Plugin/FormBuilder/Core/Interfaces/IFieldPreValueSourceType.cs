using FormBuilder.Core.Attributes;
using FormBuilder.Core.Models;

namespace FormBuilder.Core.Interfaces
{
    public interface IFieldPrevalueSourceType
    {
        string Alias { get; set; }

        Dictionary<string, SettingAttribute> Settings();

        Task<List<Prevalue>> GetPreValuesAsync(Field? field, Form? form);

        List<Exception> ValidateSettings();
    }
}