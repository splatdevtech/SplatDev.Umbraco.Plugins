
// Type: Umbraco.Forms.Web.Extensions.EnumerableExtensionMethods
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Umbraco.Forms.Web.Extensions
{
  public static class EnumerableExtensionMethods
  {
    public static bool ContainsUniqueItems<T>(this IEnumerable<T> values)
    {
      HashSet<T> set = new HashSet<T>();
      return values.All<T>((Func<T, bool>) (item => set.Add(item)));
    }
  }
}
