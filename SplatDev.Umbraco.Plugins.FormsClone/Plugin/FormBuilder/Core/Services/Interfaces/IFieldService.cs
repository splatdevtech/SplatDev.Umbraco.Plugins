using FormBuilder.Core.Models;

namespace FormBuilder.Core.Services.Interfaces
{
    public interface IFieldService
    {
        IEnumerable<string> GetDuplicates(List<Field> fields);

        bool ContainsSensitiveData(List<Field> fields);
    }
}