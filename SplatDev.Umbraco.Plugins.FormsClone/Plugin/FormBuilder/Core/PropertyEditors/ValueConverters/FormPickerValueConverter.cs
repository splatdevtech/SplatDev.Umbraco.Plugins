using FormBuilder.Core.Dto;
using FormBuilder.Core.Factory;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.DeliveryApi;

namespace FormBuilder.Core.PropertyEditors.ValueConverters
{
    [DefaultPropertyValueConverter]
    public class FormPickerValueConverter(IFormService formService, FormDtoFactory formDtoFactory) :
      PropertyValueConverterBase,
      IDeliveryApiPropertyValueConverter,
      IPropertyValueConverter,
      IDiscoverable
    {
        private readonly IFormService _formService = formService;
        private readonly FormDtoFactory _formDtoFactory = formDtoFactory;

        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias == "FormBuilder.FormPicker";

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(Guid?);

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
            return Guid.TryParse(source?.ToString(), out Guid result) ? new Guid?(result) : new Guid?();
        }

        public PropertyCacheLevel GetDeliveryApiPropertyCacheLevel(
          IPublishedPropertyType propertyType)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return PropertyCacheLevel.Snapshot;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public Type GetDeliveryApiPropertyValueType(IPublishedPropertyType propertyType) => typeof(DeliveryApiFormDto);

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
            Guid id = (Guid)inter;
            if (!expanding)
                return new DeliveryApiFormDto() { Id = id };
            Form? form = _formService.Get(id);
            if (form is null)
                return new DeliveryApiFormDto() { Id = id };
            FormDto formDto = _formDtoFactory.BuildFormDefinitionDto(form);
            return new DeliveryApiFormDto()
            {
                Id = id,
                Form = formDto
            };
        }
    }
}