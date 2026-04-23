
// Type: Umbraco.Forms.Core.Providers.CollectionBuilderHelper
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Umbraco.Forms.Core.Providers
{
  public static class CollectionBuilderHelper
  {
    public static Func<IEnumerable<T>> GetItemsWithHighestPriority<T>(T[] fieldTypes) where T : ProviderBase
    {
      Type[] first = fieldTypes != null ? ((IEnumerable<T>) fieldTypes).Select<T, Type>((Func<T, Type>) (x => x.GetType())).ToArray<Type>() : throw new ArgumentNullException(nameof (fieldTypes));
      List<T> result = new List<T>();
      foreach (T fieldType in fieldTypes)
      {
        T current = fieldType;
        Type type = ((T) current).GetType();
        if (!((IEnumerable<Type>) first).Except<Type>((IEnumerable<Type>) new Type[1]
        {
          type
        }).Any<Type>(new Func<Type, bool>(type.IsAssignableFrom)) || ((IEnumerable<T>) fieldTypes).Count<T>((Func<T, bool>) (x => x.Id.Equals(current.Id))) == 1)
          result.Add(current);
      }
      IGrouping<Guid, T>[] array = ((IEnumerable<T>) result).GroupBy<T, Guid>((Func<T, Guid>) (x => x.Id)).Where<IGrouping<Guid, T>>((Func<IGrouping<Guid, T>, bool>) (x => x.Count<T>() > 1)).ToArray<IGrouping<Guid, T>>();
      if (((IEnumerable<IGrouping<Guid, T>>) array).Any<IGrouping<Guid, T>>())
        throw new InvalidOperationException("Multiple FieldTypes with the same ID that is not inherited. The IDs: " + string.Join<Guid>(", ", ((IEnumerable<IGrouping<Guid, T>>) array).Select<IGrouping<Guid, T>, Guid>((Func<IGrouping<Guid, T>, Guid>) (x => x.Key))));
      return (Func<IEnumerable<T>>) (() => (IEnumerable<T>) result);
    }
  }
}
