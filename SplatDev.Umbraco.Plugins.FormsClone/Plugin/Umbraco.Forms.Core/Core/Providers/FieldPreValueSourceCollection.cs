
// Type: Umbraco.Forms.Core.Providers.FieldPreValueSourceCollection
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
  public class FieldPreValueSourceCollection : BuilderCollectionBase<FieldPreValueSourceType>
  {
    public FieldPreValueSourceCollection(Func<IEnumerable<FieldPreValueSourceType>> items)
      : base(CollectionBuilderHelper.GetItemsWithHighestPriority<FieldPreValueSourceType>(items().ToArray<FieldPreValueSourceType>()))
    {
    }

    public FieldPreValueSourceType this[Guid id]
    {
      get
      {
        FieldPreValueSourceType preValueSourceType = this.FirstOrDefault<FieldPreValueSourceType>((Func<FieldPreValueSourceType, bool>) (x => x.Id == id));
        if (preValueSourceType != null)
          return preValueSourceType;
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(55, 1);
        interpolatedStringHandler.AppendLiteral("Unable to find the Field PreValue Source with the GUID ");
        interpolatedStringHandler.AppendFormatted<Guid>(id);
        throw new ProviderException(interpolatedStringHandler.ToStringAndClear());
      }
    }
  }
}
