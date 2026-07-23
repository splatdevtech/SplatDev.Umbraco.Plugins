using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

namespace FormBuilder.Core.Services
{
    internal sealed class FieldService : IFieldService
    {
        public IEnumerable<string> GetDuplicates(List<Field> fields) => fields.GroupBy(x => x.Alias).Where(x => x.Count() > 1).Select(k => k.Key);

        public bool ContainsSensitiveData(List<Field> fields) => fields.Any(field => field.ContainsSensitiveData);
    }
}