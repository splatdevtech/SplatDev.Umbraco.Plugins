namespace UmbracoCms.CodeFirst.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    using Umbraco.Cms.Core;

    using UmbracoCms.CodeFirst.Interfaces;
    public static class ReflectionHelpers
    {
        public static IEnumerable<T> ExcludeInherited<T>(this List<T> originalList)
        {
            var exclusions = new List<T>();
            foreach (var item in originalList)
                if (!(item!.GetType()).Inherits<T>()) exclusions.Add(item);

            return exclusions;
        }

        public static IDocumentTypeChildren[] GetDocumentTypesByCreateOrder<T>(this Assembly assembly)
        {
            var documentTypes = assembly.GetInherited<IDocumentTypeChildren>();
            List<IDocumentTypeChildren> ordered = new List<IDocumentTypeChildren>();

            //populate in any order just to have the capacity filled
            foreach (var item in documentTypes) ordered.Add(item);

            foreach (var docType in documentTypes)
            {
                var instance = Activator.CreateInstance(docType.GetType());
                int? index = instance!.GetProperty("CreateOrder").Value<int?>();
                if (index.HasValue) ordered.Insert(index.Value, docType);
                else ordered.Add(docType);
            }
            return ordered.Distinct().ToArray<IDocumentTypeChildren>();
        }

        public static IEnumerable<T> GetInherited<T>(this Assembly assembly)
        {
            var types = assembly.GetTypes().Where(t => t.Inherits<T>() && !t.IsInterface);
            return (IEnumerable<T>)types;
        }

        public static IEnumerable<T> GetInherited<T>(string assemblyName = "")
        {
            var assembly = Helper.GetAssembly(assemblyName);
            var types = (IEnumerable<T>)assembly.GetTypes().Where(t => t.Inherits<T>() && !t.IsInterface);
            return types;
        }

        public static IEnumerable<T> GetInheritedTypes<T>(this Assembly assembly)
        {
            return (IEnumerable<T>)assembly.GetTypes().Where(t => t.Inherits<T>() && !t.IsInterface);
        }

        public static T[] OrderByCreateOrderProperty<T>(this IEnumerable<T> items)
        {
            Dictionary<int, T> orderedDict = new Dictionary<int, T>();
            List<T> ordered = new List<T>();
            List<T> unordered = new List<T>();
            foreach (var item in items)
            {
                var type = item!.GetType();
                var instance = Activator.CreateInstance(type);
                var index = instance!.GetProperty("CreateOrder").Value<int?>();
                if (index.HasValue) orderedDict.Add(index.Value, item);
                else unordered.Add(item);
            }
            foreach (var item in orderedDict.OrderBy(x => x.Key)) ordered.Add((T)item.Value!);
            ordered.AddRange(unordered);

            return ordered.ToArray<T>();
        }
    }
}
