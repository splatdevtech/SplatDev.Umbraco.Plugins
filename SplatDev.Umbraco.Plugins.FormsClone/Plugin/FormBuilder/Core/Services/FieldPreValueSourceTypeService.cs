using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Providers.Factories;
using FormBuilder.Core.Services.Interfaces;

namespace FormBuilder.Core.Services
{
    internal sealed class FieldPrevalueSourceTypeService(
      FieldPrevalueSourceCollectionFactory fieldPreValueSourceCollectionFactory) : IFieldPrevalueSourceTypeService
    {
        private readonly FieldPrevalueSourceCollectionFactory _fieldPreValueSourceCollectionFactory = fieldPreValueSourceCollectionFactory;

        public FieldPrevalueSourceType? GetById(Guid fieldPreValueSourceTypeId) => fieldPreValueSourceTypeId == Guid.Empty ? null : _fieldPreValueSourceCollectionFactory.GetPreValueSourceCollection()[fieldPreValueSourceTypeId];
    }
}