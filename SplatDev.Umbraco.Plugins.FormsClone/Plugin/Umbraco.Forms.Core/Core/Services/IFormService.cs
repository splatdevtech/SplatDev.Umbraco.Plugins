
// Type: Umbraco.Forms.Core.Services.IFormService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Services
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
