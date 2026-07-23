using System.ComponentModel;
using System.Reflection;

namespace FormBuilder.Core.Extensions
{
    internal static class EnumExtensions
    {
        public static string GetDescription<TEnum>(this TEnum value) where TEnum : struct, Enum
        {
            string? name = Enum.GetName(value);
            if (name is not null)
            {
                FieldInfo? field = value.GetType().GetField(name);
                if (field as object is not null)
                {
                    DescriptionAttribute? customAttribute = field.GetCustomAttribute<DescriptionAttribute>(false);
                    if (customAttribute is not null)
                        return customAttribute.Description;
                }
            }
            return value.ToString();
        }
    }
}