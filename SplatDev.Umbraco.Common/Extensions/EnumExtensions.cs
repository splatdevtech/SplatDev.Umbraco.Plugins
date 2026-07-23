using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SplatDev.Umbraco.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName<T>(this T enumValue) where T : Enum
        {
            var member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();
            if (member != null)
            {
                var displayAttr = member.GetCustomAttribute<DisplayAttribute>();
                if (displayAttr != null)
                    return displayAttr.Name ?? enumValue.ToString();
            }
            return enumValue.ToString();
        }
    }
}
