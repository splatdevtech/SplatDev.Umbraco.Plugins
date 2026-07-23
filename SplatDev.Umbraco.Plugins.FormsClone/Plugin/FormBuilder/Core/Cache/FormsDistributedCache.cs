using FormBuilder.Core.DataSources;
using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using FormBuilder.Core.Prevalues;

using static FormBuilder.Constants;

namespace FormBuilder.Core.Cache
{
    internal sealed class FormsDistributedCache(DistributedCache distributedCache) : IFormsDistributedCache
    {
        private readonly DistributedCache _distributedCache = distributedCache;

        public void RefreshFolders(IEnumerable<Folder> folders) => _distributedCache.RefreshByPayload(FormBuilderCacheKeys.FoldersDbCacheRefresherGuid, folders.DistinctBy(x => x.Id).Select(x => new JsonPayloads.FolderPayload()
        {
            Folder = x,
            DeleteFolder = false
        }));

        public void RemoveFolders(IEnumerable<Folder> folders) => _distributedCache.RefreshByPayload(FormBuilderCacheKeys.FoldersDbCacheRefresherGuid, folders.DistinctBy(x => x.Id).Select(x => new JsonPayloads.FolderPayload()
        {
            Folder = x,
            DeleteFolder = true
        }));

        public void RefreshForms(IEnumerable<Form> forms) => _distributedCache.RefreshByPayload(FormBuilderCacheKeys.FormsDbCacheRefresherGuid, forms.DistinctBy(x => x.Id).Select(x => new JsonPayloads.FormPayload()
        {
            Form = x,
            DeleteForm = false
        }));

        public void RemoveForms(IEnumerable<Form> forms) => _distributedCache.RefreshByPayload(FormBuilderCacheKeys.FormsDbCacheRefresherGuid, forms.DistinctBy(x => x.Id).Select(x => new JsonPayloads.FormPayload()
        {
            Form = x,
            DeleteForm = true
        }));

        public void RefreshWorkflows(IEnumerable<Workflow> workflows) => _distributedCache.RefreshByPayload(FormBuilderCacheKeys.WorkflowDbCacheRefresherGuid, workflows.DistinctBy(x => x.Id).Select(x => new JsonPayloads.WorkflowPayload()
        {
            Workflow = x,
            DeleteWorkflow = false
        }));

        public void RemoveWorkflows(IEnumerable<Workflow> workflows) => _distributedCache.RefreshByPayload(FormBuilderCacheKeys.WorkflowDbCacheRefresherGuid, workflows.DistinctBy(x => x.Id).Select(x => new JsonPayloads.WorkflowPayload()
        {
            Workflow = x,
            DeleteWorkflow = true
        }));

        public void RefreshAllPrevalueSources() => _distributedCache.RefreshAll(FormBuilderCacheKeys.PreValueDbCacheRefresherGuid);

        public void RefreshPrevalueSources(IEnumerable<FieldPrevalueSource> prevalueSources) => _distributedCache.RefreshByPayload(FormBuilderCacheKeys.PreValueDbCacheRefresherGuid, prevalueSources.DistinctBy(x => x.Id).Select(x => new JsonPayloads.PreValuePayload()
        {
            Prevalue = x,
            DeletePreValue = false
        }));

        public void RemovePrevalueSources(IEnumerable<FieldPrevalueSource> prevalueSources) => _distributedCache.RefreshByPayload(FormBuilderCacheKeys.PreValueDbCacheRefresherGuid, prevalueSources.DistinctBy(x => x.Id).Select(x => new JsonPayloads.PreValuePayload()
        {
            Prevalue = x,
            DeletePreValue = true
        }));

        public void RefreshDataSources(IEnumerable<FormDataSource> dataSources) => _distributedCache.RefreshByPayload(FormBuilderCacheKeys.DataSourceDbCacheRefresherGuid, dataSources.DistinctBy(x => x.Id).Select(x => new JsonPayloads.DataSourcePayload()
        {
            DataSource = x,
            DeleteDataSource = false
        }));

        public void RemoveDataSources(IEnumerable<FormDataSource> dataSources) => _distributedCache.RefreshByPayload(FormBuilderCacheKeys.DataSourceDbCacheRefresherGuid, dataSources.DistinctBy(x => x.Id).Select(x => new JsonPayloads.DataSourcePayload()
        {
            DataSource = x,
            DeleteDataSource = true
        }));
    }
}