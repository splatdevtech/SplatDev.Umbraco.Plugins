
// Type: Umbraco.Forms.Core.Providers.ValidationPatternCollection
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Composing;
using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Providers
{
  public class ValidationPatternCollection : BuilderCollectionBase<IValidationPattern>
  {
    public ValidationPatternCollection(Func<IEnumerable<IValidationPattern>> items)
      : base(items)
    {
    }
  }
}
