
// Type: Umbraco.Forms.Core.Services.IWorkflowExecutionService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
  public interface IWorkflowExecutionService
  {
    Task<WorkflowExecutionResult> ExecuteWorkflowsAsync(
      Record record,
      Form form,
      FormState state,
      bool editMode);

    Task<WorkflowExecutionResult> ExecuteWorkflowsAsync(
      List<Workflow> workflows,
      Record record,
      Form form,
      FormState state);
  }
}
