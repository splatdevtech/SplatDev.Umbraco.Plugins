using FormBuilder.Core.Models;

namespace FormBuilder.Core.Services.Interfaces
{
    public interface IFormService : IBaseService<Form, FormEntity>
    {
        IEnumerable<Form> SearchForms(
          string query,
          int pageIndex,
          int pageSize,
          out long totalFound);

        Form? Get(string name);

        Form? GetFromCache(string name);

        bool ContainsSensitiveData(Form form);

        bool FormExists(string formName);

        IEnumerable<FormSlim> GetSlim();

        FormSlim? GetSlim(Guid id);

        IEnumerable<Form> GetAtRoot();

        IEnumerable<FormSlim> GetAtRootSlim();

        IEnumerable<Form> GetFromFolder(Guid parentFolderId);

        IEnumerable<FormSlim> GetFromFolderSlim(Guid parentFolderId);
    }
}