using FormBuilder.Core.Models;
using FormBuilder.Core.Options;

using System.Text.Json;

using Umbraco.Extensions;

namespace FormBuilder.Core.Extensions
{
    public static class FormViewModelExtensions
    {
        /// <summary>
        /// Extension method for getting all fields guid and value pairs as a JSON string.
        /// </summary>
        public static string AllFieldsAsJson(this FormViewModel model)
        {
            Dictionary<string, string> dictionary = [];
            foreach (PageViewModel page in model.Pages)
            {
                foreach (FieldsetViewModel fieldset in (IEnumerable<FieldsetViewModel>)page.Fieldsets)
                {
                    foreach (FieldsetContainerViewModel container in (IEnumerable<FieldsetContainerViewModel>)fieldset.Containers)
                    {
                        foreach (FieldViewModel field in (IEnumerable<FieldViewModel>)container.Fields)
                            dictionary.Add(field.Name, !string.IsNullOrWhiteSpace(field.ValueAsHtmlString.ToHtmlString()) ? field.ValueAsHtmlString.ToHtmlString() : string.Empty);
                    }
                }
            }
            return JsonSerializer.Serialize(dictionary, FormsJsonSerializerOptions.Default);
        }

        /// <summary>
        /// Extension method for serializing the fieldset conditions into a JSON string.
        /// </summary>
        public static string FieldsetConditionsAsJson(this FormViewModel model) => JsonSerializer.Serialize(model.FieldsetConditions, FormsJsonSerializerOptions.Default);

        /// <summary>
        /// Extension method for serializing the field conditions into a JSON string
        /// </summary>
        public static string FieldConditionsAsJson(this FormViewModel model) => JsonSerializer.Serialize(model.FieldConditions, FormsJsonSerializerOptions.Default);
    }
}