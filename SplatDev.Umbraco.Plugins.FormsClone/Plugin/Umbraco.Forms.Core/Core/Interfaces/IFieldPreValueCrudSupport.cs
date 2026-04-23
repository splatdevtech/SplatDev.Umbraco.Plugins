
// Type: Umbraco.Forms.Core.Interfaces.IFieldPreValueCrudSupport
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Interfaces
{
  public interface IFieldPreValueCrudSupport
  {
    PreValue InsertValue(PreValue preValue, Field field, Form form);

    PreValue UpdateValue(PreValue preValue, Field field, Form form);

    bool DeleteValue(PreValue preValue, Field field, Form form);

    void SortValues(Guid fieldId, List<PreValue> values, Field field, Form form);
  }
}
