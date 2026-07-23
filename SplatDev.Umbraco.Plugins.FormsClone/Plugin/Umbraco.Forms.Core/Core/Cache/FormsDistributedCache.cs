
// Type: Umbraco.Forms.Core.Cache.FormsDistributedCache
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Cache;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Cache
{
  internal sealed class FormsDistributedCache : IFormsDistributedCache
  {
    private readonly DistributedCache _distributedCache;

    public FormsDistributedCache(DistributedCache distributedCache) => this._distributedCache = distributedCache;

    public void RefreshFolders(IEnumerable<Folder> folders) => this._distributedCache.RefreshByPayload<JsonPayloads.FolderPayload>(CacheKeys.FoldersDbCacheRefresherGuid, folders.DistinctBy<Folder, Guid>((Func<Folder, Guid>) (x => x.Id)).Select<Folder, JsonPayloads.FolderPayload>((Func<Folder, JsonPayloads.FolderPayload>) (x => new JsonPayloads.FolderPayload()
    {
      Folder = x,
      DeleteFolder = false
    })));

    public void RemoveFolders(IEnumerable<Folder> folders) => this._distributedCache.RefreshByPayload<JsonPayloads.FolderPayload>(CacheKeys.FoldersDbCacheRefresherGuid, folders.DistinctBy<Folder, Guid>((Func<Folder, Guid>) (x => x.Id)).Select<Folder, JsonPayloads.FolderPayload>((Func<Folder, JsonPayloads.FolderPayload>) (x => new JsonPayloads.FolderPayload()
    {
      Folder = x,
      DeleteFolder = true
    })));

    public void RefreshForms(IEnumerable<Form> forms) => this._distributedCache.RefreshByPayload<JsonPayloads.FormPayload>(CacheKeys.FormsDbCacheRefresherGuid, forms.DistinctBy<Form, Guid>((Func<Form, Guid>) (x => x.Id)).Select<Form, JsonPayloads.FormPayload>((Func<Form, JsonPayloads.FormPayload>) (x => new JsonPayloads.FormPayload()
    {
      Form = x,
      DeleteForm = false
    })));

    public void RemoveForms(IEnumerable<Form> forms) => this._distributedCache.RefreshByPayload<JsonPayloads.FormPayload>(CacheKeys.FormsDbCacheRefresherGuid, forms.DistinctBy<Form, Guid>((Func<Form, Guid>) (x => x.Id)).Select<Form, JsonPayloads.FormPayload>((Func<Form, JsonPayloads.FormPayload>) (x => new JsonPayloads.FormPayload()
    {
      Form = x,
      DeleteForm = true
    })));

    public void RefreshWorkflows(IEnumerable<Workflow> workflows) => this._distributedCache.RefreshByPayload<JsonPayloads.WorkflowPayload>(CacheKeys.WorkflowDbCacheRefresherGuid, workflows.DistinctBy<Workflow, Guid>((Func<Workflow, Guid>) (x => x.Id)).Select<Workflow, JsonPayloads.WorkflowPayload>((Func<Workflow, JsonPayloads.WorkflowPayload>) (x => new JsonPayloads.WorkflowPayload()
    {
      Workflow = x,
      DeleteWorkflow = false
    })));

    public void RemoveWorkflows(IEnumerable<Workflow> workflows) => this._distributedCache.RefreshByPayload<JsonPayloads.WorkflowPayload>(CacheKeys.WorkflowDbCacheRefresherGuid, workflows.DistinctBy<Workflow, Guid>((Func<Workflow, Guid>) (x => x.Id)).Select<Workflow, JsonPayloads.WorkflowPayload>((Func<Workflow, JsonPayloads.WorkflowPayload>) (x => new JsonPayloads.WorkflowPayload()
    {
      Workflow = x,
      DeleteWorkflow = true
    })));

    public void RefreshAllPrevalueSources() => this._distributedCache.RefreshAll(CacheKeys.PreValueDbCacheRefresherGuid);

    public void RefreshPrevalueSources(IEnumerable<FieldPreValueSource> prevalueSources) => this._distributedCache.RefreshByPayload<JsonPayloads.PreValuePayload>(CacheKeys.PreValueDbCacheRefresherGuid, prevalueSources.DistinctBy<FieldPreValueSource, Guid>((Func<FieldPreValueSource, Guid>) (x => x.Id)).Select<FieldPreValueSource, JsonPayloads.PreValuePayload>((Func<FieldPreValueSource, JsonPayloads.PreValuePayload>) (x => new JsonPayloads.PreValuePayload()
    {
      PreValue = x,
      DeletePreValue = false
    })));

    public void RemovePrevalueSources(IEnumerable<FieldPreValueSource> prevalueSources) => this._distributedCache.RefreshByPayload<JsonPayloads.PreValuePayload>(CacheKeys.PreValueDbCacheRefresherGuid, prevalueSources.DistinctBy<FieldPreValueSource, Guid>((Func<FieldPreValueSource, Guid>) (x => x.Id)).Select<FieldPreValueSource, JsonPayloads.PreValuePayload>((Func<FieldPreValueSource, JsonPayloads.PreValuePayload>) (x => new JsonPayloads.PreValuePayload()
    {
      PreValue = x,
      DeletePreValue = true
    })));

    public void RefreshDataSources(IEnumerable<FormDataSource> dataSources) => this._distributedCache.RefreshByPayload<JsonPayloads.DataSourcePayload>(CacheKeys.DataSourceDbCacheRefresherGuid, dataSources.DistinctBy<FormDataSource, Guid>((Func<FormDataSource, Guid>) (x => x.Id)).Select<FormDataSource, JsonPayloads.DataSourcePayload>((Func<FormDataSource, JsonPayloads.DataSourcePayload>) (x => new JsonPayloads.DataSourcePayload()
    {
      DataSource = x,
      DeleteDataSource = false
    })));

    public void RemoveDataSources(IEnumerable<FormDataSource> dataSources) => this._distributedCache.RefreshByPayload<JsonPayloads.DataSourcePayload>(CacheKeys.DataSourceDbCacheRefresherGuid, dataSources.DistinctBy<FormDataSource, Guid>((Func<FormDataSource, Guid>) (x => x.Id)).Select<FormDataSource, JsonPayloads.DataSourcePayload>((Func<FormDataSource, JsonPayloads.DataSourcePayload>) (x => new JsonPayloads.DataSourcePayload()
    {
      DataSource = x,
      DeleteDataSource = true
    })));
  }
}
