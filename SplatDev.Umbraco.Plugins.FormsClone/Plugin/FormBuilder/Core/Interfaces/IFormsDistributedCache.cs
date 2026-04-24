using FormBuilder.Core.DataSources;
using FormBuilder.Core.Models;
using FormBuilder.Core.Prevalues;

using Folder = FormBuilder.Core.Models.Folder;

namespace FormBuilder.Core.Interfaces
{
    public interface IFormsDistributedCache
    {
        void RefreshFolders(IEnumerable<Folder> folders);

        void RemoveFolders(IEnumerable<Folder> folders);

        void RefreshForms(IEnumerable<Form> forms);

        void RemoveForms(IEnumerable<Form> forms);

        void RefreshPrevalueSources(IEnumerable<FieldPrevalueSource> prevalueSources);

        void RemovePrevalueSources(IEnumerable<FieldPrevalueSource> prevalueSources);

        void RefreshWorkflows(IEnumerable<Workflow> workflows);

        void RemoveWorkflows(IEnumerable<Workflow> workflows);

        void RefreshDataSources(IEnumerable<FormDataSource> dataSources);

        void RemoveDataSources(IEnumerable<FormDataSource> dataSources);
    }
}