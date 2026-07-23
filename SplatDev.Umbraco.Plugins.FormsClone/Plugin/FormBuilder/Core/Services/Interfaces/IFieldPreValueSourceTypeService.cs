using FormBuilder.Core.Prevalues;

namespace FormBuilder.Core.Services.Interfaces
{
    public interface IFieldPrevalueSourceTypeService
    {
        FieldPrevalueSourceType? GetById(Guid fieldPreValueSourceTypeId);
    }
}