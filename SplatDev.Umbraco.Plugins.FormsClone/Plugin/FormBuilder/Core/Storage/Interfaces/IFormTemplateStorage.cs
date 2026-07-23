using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;

namespace FormBuilder.Core.Storage.Interfaces
{
    public interface IFormTemplateStorage
    {
        IEnumerable<FormTemplateBase> GetAllTemplates();

        Form? GetTemplate(string alias);

        (Form? Form, IDictionary<FormState, IEnumerable<Workflow>> Workflows) GetTemplateWithWorkflows(
          string alias);
    }
}