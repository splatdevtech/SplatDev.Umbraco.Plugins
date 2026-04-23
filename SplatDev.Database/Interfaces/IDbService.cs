namespace SplatDev.Database.Interfaces
{
    using System.Threading.Tasks;
    public interface IDbService<T>
    {
        Task<T> GetById(int id);
        Task<bool> Update(T data, int primaryKeyValue);
        Task<bool> Delete(int id);
        Task<bool> Insert(T data);
        Task<bool> Exists(int id);
    }
}
