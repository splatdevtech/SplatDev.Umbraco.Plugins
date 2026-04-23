using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace SplatDev.Umbraco.Common.Extensions
{
    public static class PublishedContentPropertyExtensions
    {
        /// <summary>
        /// Returns value of the property with given <paramref name="propertyName"/> from the <paramref name="source"/>.
        /// </summary>
        /// <remarks>
        /// This method is exactly the same as Umbraco's own <see cref="Umbraco.Web.PublishedContentExtensions.Value{T}(IPublishedContent,string,string,string,Fallback,T)"/>,
        /// except this one will deduce property name from the caller's context, if name is omitted.
        /// </remarks>
        /// <typeparam name="T">Expected type of the property value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Property name.</param>
        /// <returns>Value of the property with given <paramref name="propertyName"/>.</returns>
        public static T? GetPropertyValue<T>(this IPublishedContent source, [CallerMemberName] string propertyName = "")
            => source.Value<T>(propertyName);

        /// <summary>
        /// Returns value of the property with given <paramref name="propertyName"/> from the <paramref name="source"/>,
        /// or provided <paramref name="defaultValue"/> if property is not found or value is not assigned to it.
        /// </summary>
        /// <remarks>
        /// This method is exactly the same as Umbraco's own <see cref="Umbraco.Web.PublishedContentExtensions.Value{T}(IPublishedContent,string,string,string,Fallback,T)"/>,
        /// except this one will deduce property name from the caller's context, if name is omitted.
        /// </remarks>
        /// <typeparam name="T">Expected type of the property value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="defaultValue">Default value that will be returned if property or its value are not found.</param>
        /// <param name="propertyName">Property name.</param>
        /// <returns>Value of the property with given <paramref name="propertyName"/> or <paramref name="defaultValue"/> if property or its value are not found.</returns>
        public static T? GetPropertyWithDefaultValue<T>(this IPublishedContent source, T defaultValue, [CallerMemberName] string propertyName = "")
            => source.Value(propertyName, source.GetCultureFromDomains(), null, Fallback.ToDefaultValue, defaultValue);

        /// <summary>
        /// Returns value of the property with given <paramref name="propertyName"/> from the <paramref name="source"/>,
        /// or creates default value by using provided <paramref name="defaultValueFactory"/> if property is not found or value is not assigned to it.
        /// </summary>
        /// <typeparam name="T">Expected type of the property value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="defaultValueFactory">Default value factory that will be used if property or its value are not found.</param>
        /// <param name="propertyName">Property name.</param>
        /// <returns>Value of the property with given <paramref name="propertyName"/> or value returned by <paramref name="defaultValueFactory"/> if property or its value are not found.</returns>
        public static T? GetPropertyWithDefaultValue<T>(this IPublishedContent source, Func<T> defaultValueFactory, [CallerMemberName] string propertyName = "")
            => source.HasValue(propertyName) ? source.Value<T>(propertyName) : defaultValueFactory();
    }
}
