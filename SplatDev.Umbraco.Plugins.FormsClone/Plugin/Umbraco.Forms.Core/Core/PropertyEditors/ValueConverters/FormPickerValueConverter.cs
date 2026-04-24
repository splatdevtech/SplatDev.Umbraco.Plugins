
// Type: Umbraco.Forms.Core.PropertyEditors.ValueConverters.FormPickerValueConverter
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.DeliveryApi;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Models.DeliveryApi;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.PropertyEditors.ValueConverters
{
  [DefaultPropertyValueConverter]
  public class FormPickerValueConverter : 
    PropertyValueConverterBase,
    IDeliveryApiPropertyValueConverter,
    IPropertyValueConverter,
    IDiscoverable
  {
    private readonly IFormService _formService;
    private readonly FormDtoFactory _formDtoFactory;

    public FormPickerValueConverter(IFormService formService, FormDtoFactory formDtoFactory)
    {
      this._formService = formService;
      this._formDtoFactory = formDtoFactory;
    }

    public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias == "UmbracoForms.FormPicker";

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof (Guid?);

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
      Guid result;
      return (object) (Guid.TryParse(source?.ToString(), out result) ? new Guid?(result) : null);
    }

    public PropertyCacheLevel GetDeliveryApiPropertyCacheLevel(
      IPublishedPropertyType propertyType)
    {
      return PropertyCacheLevel.Snapshot;
    }

    public Type GetDeliveryApiPropertyValueType(IPublishedPropertyType propertyType) => typeof (DeliveryApiFormDto);

    public object? ConvertIntermediateToDeliveryApiObject(
      IPublishedElement owner,
      IPublishedPropertyType propertyType,
      PropertyCacheLevel referenceCacheLevel,
      object? inter,
      bool preview,
      bool expanding)
    {
      if (inter == null)
        return (object) null;
      Guid id = (Guid) inter;
      if (!expanding)
        return (object) new DeliveryApiFormDto() { Id = id };
      Form form = this._formService.Get(id);
      if (form == null)
        return (object) new DeliveryApiFormDto() { Id = id };
      FormDto formDto = this._formDtoFactory.BuildFormDefinitionDto(form);
      return (object) new DeliveryApiFormDto()
      {
        Id = id,
        Form = formDto
      };
    }
  }
}
