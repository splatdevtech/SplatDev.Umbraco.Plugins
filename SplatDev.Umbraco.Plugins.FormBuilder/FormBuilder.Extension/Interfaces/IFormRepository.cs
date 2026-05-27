using FormBuilder.Extension.Entities;
using FormBuilder.Extension.Models;

namespace FormBuilder.Extension.Interfaces
{
    public interface IFormRepository
    {
        Task<Form?> GetByIdAsync(int formId);
        Task<Form> CreateAsync(Form form);
        Task<Form> UpdateAsync(Form form);
        Task DeleteAsync(int formId);
        Task<IEnumerable<Form>> GetAllAsync();
        Task<bool> HasSubmissionsAsync(int formId);
        Task UpdateOrderAsync(int formId, Dictionary<int, int> fieldOrder);
        Task<Form> CreateSubmissionAsync(FormSubmissionModel model);
    }
}