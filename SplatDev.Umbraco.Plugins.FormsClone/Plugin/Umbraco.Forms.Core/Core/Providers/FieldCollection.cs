
// Type: Umbraco.Forms.Core.Providers.FieldCollection
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Composing;


#nullable enable
namespace Umbraco.Forms.Core.Providers
{
  public class FieldCollection : BuilderCollectionBase<FieldType>
  {
    public FieldCollection(Func<IEnumerable<FieldType>> items)
      : base(CollectionBuilderHelper.GetItemsWithHighestPriority<FieldType>(items().ToArray<FieldType>()))
    {
    }

    public FieldType? this[Guid id] => this.FirstOrDefault<FieldType>((Func<FieldType, bool>) (x => x.Id == id)) ?? (FieldType) null;
  }
}
