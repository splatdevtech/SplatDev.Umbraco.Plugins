using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Services.Interfaces;

namespace FormBuilder.Core.Services
{
    internal sealed class FieldPrevalueSourceService : IFieldPrevalueSourceService
    {
        public static readonly Guid DefaultFieldPrevalueSourceId = new("9FFD217D-C8DF-4a59-9C9A-690A6F389DC4");
        private readonly IPrevalueSourceService _prevalueSourceService;

        public FieldPrevalueSourceService(IPrevalueSourceService prevalueSourceService) => _prevalueSourceService = prevalueSourceService;

        public FieldPrevalueSource GetDefaultProvider() => new()
        {
            Name = "Default Provider",
            Id = DefaultFieldPrevalueSourceId,
            Settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        };

        public FieldPrevalueSource? GetById(Guid fieldPreValueSourceId) => fieldPreValueSourceId != DefaultFieldPrevalueSourceId && fieldPreValueSourceId != Guid.Empty ? _prevalueSourceService.Get(fieldPreValueSourceId) : null;
    }
}