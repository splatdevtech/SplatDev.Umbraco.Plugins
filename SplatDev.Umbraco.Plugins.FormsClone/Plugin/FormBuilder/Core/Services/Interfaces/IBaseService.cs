using FormBuilder.Core.Interfaces;

using Umbraco.Cms.Core.Models.Entities;

namespace FormBuilder.Core.Services.Interfaces
{
    public interface IBaseService<TType, TEntity>
      where TType : IType
      where TEntity : IEntity
    {
        void Delete(Guid id);

        bool Delete(TType item);

        IEnumerable<TType> Get();

        TType? Get(Guid id);

        bool Exists(Guid id);

        IEnumerable<TType> Get(Guid[] ids);

        TType? Insert(TType item);

        IEnumerable<TType> Update(IEnumerable<TType> items);

        TType Update(TType item);

        TType Update(TType item, Dictionary<string, object?> additionalData);
    }
}