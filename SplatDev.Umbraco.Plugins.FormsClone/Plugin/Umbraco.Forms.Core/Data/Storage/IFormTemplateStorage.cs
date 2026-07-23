
// Type: Umbraco.Forms.Data.Storage.IFormTemplateStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Data.Storage
{
  public interface IFormTemplateStorage
  {
    IEnumerable<FormTemplateBase> GetAllTemplates();

    Form? GetTemplate(string alias);

    (Form? Form, IDictionary<FormState, IEnumerable<Workflow>> Workflows) GetTemplateWithWorkflows(
      string alias);
  }
}
