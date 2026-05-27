using FormBuilder.Extension.Models;

namespace FormBuilder.Extension.Interfaces
{
    public interface IFormSubmissionValidator
    {
        Task<ValidationResultModel> ValidateAsync(FormSubmissionModel model);
    }
}
