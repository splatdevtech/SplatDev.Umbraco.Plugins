
// Type: Umbraco.Forms.Core.Persistence.Repositories.IWorkflowRepository
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Persistence;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Repositories
{
  public interface IWorkflowRepository : 
    IReadWriteQueryRepository<Guid, WorkflowEntity>,
    IReadRepository<Guid, WorkflowEntity>,
    IRepository,
    IWriteRepository<WorkflowEntity>,
    IQueryRepository<WorkflowEntity>
  {
    IEnumerable<WorkflowEntity> GetFor(Form form);

    IEnumerable<WorkflowEntitySlim> GetForSlim(Guid formId);

    IEnumerable<WorkflowEntitySlim> GetForSlim(Form form);
  }
}
