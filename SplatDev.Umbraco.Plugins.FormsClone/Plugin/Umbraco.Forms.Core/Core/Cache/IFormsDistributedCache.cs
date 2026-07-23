
// Type: Umbraco.Forms.Core.Cache.IFormsDistributedCache
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Cache
{
  public interface IFormsDistributedCache
  {
    void RefreshFolders(IEnumerable<Folder> folders);

    void RemoveFolders(IEnumerable<Folder> folders);

    void RefreshForms(IEnumerable<Form> forms);

    void RemoveForms(IEnumerable<Form> forms);

    void RefreshPrevalueSources(IEnumerable<FieldPreValueSource> prevalueSources);

    void RemovePrevalueSources(IEnumerable<FieldPreValueSource> prevalueSources);

    void RefreshWorkflows(IEnumerable<Workflow> workflows);

    void RemoveWorkflows(IEnumerable<Workflow> workflows);

    void RefreshDataSources(IEnumerable<FormDataSource> dataSources);

    void RemoveDataSources(IEnumerable<FormDataSource> dataSources);
  }
}
