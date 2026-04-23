
// Type: Umbraco.Forms.Core.Providers.RecordSetActionCollection
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Umbraco.Cms.Core.Composing;
using Umbraco.Forms.Core.Exceptions;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Providers
{
  public class RecordSetActionCollection : BuilderCollectionBase<RecordSetActionType>
  {
    public RecordSetActionCollection(Func<IEnumerable<RecordSetActionType>> items)
      : base(CollectionBuilderHelper.GetItemsWithHighestPriority<RecordSetActionType>(items().ToArray<RecordSetActionType>()))
    {
    }

    public RecordSetActionType this[Guid id]
    {
      get
      {
        RecordSetActionType recordSetActionType = this.FirstOrDefault<RecordSetActionType>((Func<RecordSetActionType, bool>) (x => x.Id == id));
        if (recordSetActionType != null)
          return recordSetActionType;
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 1);
        interpolatedStringHandler.AppendLiteral("Unable to find the RecordSet Action with the GUID ");
        interpolatedStringHandler.AppendFormatted<Guid>(id);
        throw new ProviderException(interpolatedStringHandler.ToStringAndClear());
      }
    }
  }
}
