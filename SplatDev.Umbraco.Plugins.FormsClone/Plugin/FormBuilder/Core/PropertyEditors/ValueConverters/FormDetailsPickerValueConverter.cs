using FormBuilder.Core.Dto;
using FormBuilder.Core.Factory;
using FormBuilder.Core.Models;
using FormBuilder.Core.PropertyEditors.Models;
using FormBuilder.Core.Services.Interfaces;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.DeliveryApi;
using Umbraco.Cms.Core.Serialization;

namespace FormBuilder.Core.PropertyEditors.ValueConverters
{
    [DefaultPropertyValueConverter]
    public class FormDetailsPickerValueConverter(
      IFormService formService,
      IJsonSerializer jsonSerializer,
      FormDtoFactory formDtoFactory) :
      PropertyValueConverterBase,
      IDeliveryApiPropertyValueConverter,
      IPropertyValueConverter,
      IDiscoverable
    {
        private readonly IFormService _formService = formService;
        private readonly IJsonSerializer _jsonSerializer = jsonSerializer;
        private readonly FormDtoFactory _formDtoFactory = formDtoFactory;

        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias == "FormBuilder.FormDetailsPicker";

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(FormDetails);

        public override PropertyCacheLevel GetPropertyCacheLevel(
          IPublishedPropertyType propertyType)
        {
            return PropertyCacheLevel.Element;
        }

        public override object? ConvertSourceToIntermediate(
          IPublishedElement owner,
          IPublishedPropertyType propertyType,
          object? source,
          bool preview)
        {
            string? input = source?.ToString();
            return string.IsNullOrWhiteSpace(input) ? null : _jsonSerializer.Deserialize<FormDetails>(input);
        }

        public PropertyCacheLevel GetDeliveryApiPropertyCacheLevel(
          IPublishedPropertyType propertyType)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return PropertyCacheLevel.Snapshot;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public Type GetDeliveryApiPropertyValueType(IPublishedPropertyType propertyType) => typeof(DeliveryApiFormDetailsDto);

        public object? ConvertIntermediateToDeliveryApiObject(
          IPublishedElement owner,
          IPublishedPropertyType propertyType,
          PropertyCacheLevel referenceCacheLevel,
          object? inter,
          bool preview,
          bool expanding)
        {
            if (inter is null)
                return null;
            FormDetails formDetails = (FormDetails)inter;
            Guid? formId = formDetails.FormId;
            if (!formId.HasValue)
                return null;
            DeliveryApiFormDetailsDto apiFormDetailsDto = new();
            formId = formDetails.FormId;
            if (formId is null) return null;
            apiFormDetailsDto.FormId = formId.Value;
            apiFormDetailsDto.Theme = formDetails.Theme;
            apiFormDetailsDto.RedirectToPageId = formDetails.RedirectToPageId;
            DeliveryApiFormDetailsDto deliveryApiObject = apiFormDetailsDto;
            if (!expanding)
                return deliveryApiObject;
            IFormService formService = _formService;
            formId = formDetails.FormId;
            if (formId is null) return null;
            Guid? id = formId;
            if (id is null) return null;
            Form? form = formService.Get(id.Value);
            if (form is null)
                return deliveryApiObject;
            FormDto formDto = _formDtoFactory.BuildFormDefinitionDto(form);
            deliveryApiObject.Form = formDto;
            return deliveryApiObject;
        }
    }
}