
// Type: Umbraco.Forms.Core.Providers.WorkflowCollection
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Umbraco.Cms.Core.Composing;
using Umbraco.Forms.Core.Exceptions;


#nullable enable
namespace Umbraco.Forms.Core.Providers
{
  public class WorkflowCollection : BuilderCollectionBase<WorkflowType>
  {
    public WorkflowCollection(Func<IEnumerable<WorkflowType>> items)
      : base(CollectionBuilderHelper.GetItemsWithHighestPriority<WorkflowType>(items().ToArray<WorkflowType>()))
    {
    }

    public WorkflowType this[Guid id]
    {
      get
      {
        WorkflowType workflowType = this.FirstOrDefault<WorkflowType>((Func<WorkflowType, bool>) (x => x.Id == id));
        if (workflowType != null)
          return workflowType;
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 1);
        interpolatedStringHandler.AppendLiteral("Unable to find the Workflow with the GUID ");
        interpolatedStringHandler.AppendFormatted<Guid>(id);
        throw new ProviderException(interpolatedStringHandler.ToStringAndClear());
      }
    }
  }
}
