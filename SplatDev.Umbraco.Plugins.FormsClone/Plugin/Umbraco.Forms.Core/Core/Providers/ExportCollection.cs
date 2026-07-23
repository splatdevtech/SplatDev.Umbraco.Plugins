
// Type: Umbraco.Forms.Core.Providers.ExportCollection
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
  public class ExportCollection : BuilderCollectionBase<ExportType>
  {
    public ExportCollection(Func<IEnumerable<ExportType>> items)
      : base(CollectionBuilderHelper.GetItemsWithHighestPriority<ExportType>(items().ToArray<ExportType>()))
    {
    }

    public ExportType this[Guid id]
    {
      get
      {
        ExportType exportType = this.FirstOrDefault<ExportType>((Func<ExportType, bool>) (x => x.Id == id));
        if (exportType != null)
          return exportType;
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 1);
        interpolatedStringHandler.AppendLiteral("Unable to find the Export Type with the GUID ");
        interpolatedStringHandler.AppendFormatted<Guid>(id);
        throw new ProviderException(interpolatedStringHandler.ToStringAndClear());
      }
    }
  }
}
