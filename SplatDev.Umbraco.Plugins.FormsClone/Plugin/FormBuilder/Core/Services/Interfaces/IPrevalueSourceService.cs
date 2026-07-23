using FormBuilder.Core.Models;
using FormBuilder.Core.Prevalues;

namespace FormBuilder.Core.Services.Interfaces
{
    public interface IPrevalueSourceService : IBaseService<FieldPrevalueSource, PrevalueSourceEntity>
    {
        IEnumerable<FieldPrevalueSourceSlim> GetSlim(params Guid[] ids);
    }
}