
// Type: Umbraco.Forms.Core.Extensions.EnumExtensions
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.ComponentModel;
using System.Reflection;


#nullable enable
namespace Umbraco.Forms.Core.Extensions
{
  internal static class EnumExtensions
  {
    public static string GetDescription<TEnum>(this TEnum value) where TEnum : struct, Enum
    {
      string name = Enum.GetName<TEnum>(value);
      if (name != null)
      {
        FieldInfo field = value.GetType().GetField(name);
        if ((object) field != null)
        {
          DescriptionAttribute customAttribute = field.GetCustomAttribute<DescriptionAttribute>(false);
          if (customAttribute != null)
            return customAttribute.Description;
        }
      }
      return value.ToString();
    }
  }
}
