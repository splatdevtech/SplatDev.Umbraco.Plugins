
// Type: Umbraco.Forms.Core.PropertyEditors.ValueConverters.FormDetailsPickerValueConverter
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.DeliveryApi;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Models.DeliveryApi;
using Umbraco.Forms.Core.PropertyEditors.Models;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.PropertyEditors.ValueConverters
{
  [DefaultPropertyValueConverter]
  public class FormDetailsPickerValueConverter : 
    PropertyValueConverterBase,
    IDeliveryApiPropertyValueConverter,
    IPropertyValueConverter,
    IDiscoverable
  {
    private readonly IFormService _formService;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly FormDtoFactory _formDtoFactory;

    public FormDetailsPickerValueConverter(
      IFormService formService,
      IJsonSerializer jsonSerializer,
      FormDtoFactory formDtoFactory)
    {
      this._formService = formService;
      this._jsonSerializer = jsonSerializer;
      this._formDtoFactory = formDtoFactory;
    }

    public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias == "UmbracoForms.FormDetailsPicker";

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof (FormDetails);

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
      string input = source?.ToString();
      return string.IsNullOrWhiteSpace(input) ? (object) null : (object) this._jsonSerializer.Deserialize<FormDetails>(input);
    }

    public PropertyCacheLevel GetDeliveryApiPropertyCacheLevel(
      IPublishedPropertyType propertyType)
    {
      return PropertyCacheLevel.Snapshot;
    }

    public Type GetDeliveryApiPropertyValueType(IPublishedPropertyType propertyType) => typeof (DeliveryApiFormDetailsDto);

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
      FormDetails formDetails = (FormDetails) inter;
      Guid? formId = formDetails.FormId;
      if (!formId.HasValue)
        return (object) null;
      DeliveryApiFormDetailsDto apiFormDetailsDto = new DeliveryApiFormDetailsDto();
      formId = formDetails.FormId;
      apiFormDetailsDto.FormId = formId.Value;
      apiFormDetailsDto.Theme = formDetails.Theme;
      apiFormDetailsDto.RedirectToPageId = formDetails.RedirectToPageId;
      DeliveryApiFormDetailsDto deliveryApiObject = apiFormDetailsDto;
      if (!expanding)
        return (object) deliveryApiObject;
      IFormService formService = this._formService;
      formId = formDetails.FormId;
      Guid id = formId.Value;
      Form form = formService.Get(id);
      if (form == null)
        return (object) deliveryApiObject;
      FormDto formDto = this._formDtoFactory.BuildFormDefinitionDto(form);
      deliveryApiObject.Form = formDto;
      return (object) deliveryApiObject;
    }
  }
}
