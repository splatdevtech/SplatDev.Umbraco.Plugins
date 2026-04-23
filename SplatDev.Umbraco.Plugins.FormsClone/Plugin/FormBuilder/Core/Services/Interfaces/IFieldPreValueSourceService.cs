using FormBuilder.Core.Prevalues;

namespace FormBuilder.Core.Services.Interfaces
{
    public interface IFieldPrevalueSourceService
    {
        FieldPrevalueSource GetDefaultProvider();

        FieldPrevalueSource? GetById(Guid fieldPreValueSourceId);
    }
}