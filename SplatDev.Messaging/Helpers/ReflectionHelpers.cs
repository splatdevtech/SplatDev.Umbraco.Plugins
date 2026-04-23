namespace SplatDev.Messaging.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    public static class ReflectionHelpers
    {
        public static IEnumerable<Type> GetInheritedTypes<T>(this Assembly assembly)
        {
            return assembly.GetTypes().Where(t => t.Inherits<T>() && !t.IsInterface);
        }

        public static List<Type> GetInherited<T>(string assemblyName = "")
        {
            var assembly = Assembly.Load(assemblyName);

            var types = assembly.GetTypes().Where(t => t.Inherits<T>() && !t.IsInterface).ToList();
            return types;
        }

        public static List<Type> GetInherited<T>(this Assembly assembly)
        {
            var types = assembly.GetTypes().Where(t => t.Inherits<T>() && !t.IsInterface).ToList();
            return types;
        }

        public static List<Type> ExcludeInherited<T>(this List<Type> originalList)
        {
            var exclusions = new List<Type>();
            foreach (var item in originalList)
                if (!item.Inherits<T>()) exclusions.Add(item);

            return exclusions;
        }

        private static bool Inherits<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }
    }
}