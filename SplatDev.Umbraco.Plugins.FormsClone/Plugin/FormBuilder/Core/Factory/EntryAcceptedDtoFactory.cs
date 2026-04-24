using FormBuilder.Core.Dto;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Services.Interfaces;

using System.Collections;

using Umbraco.Cms.Core.DeliveryApi;

using Umbraco.Cms.Core.PublishedCache;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Core.Factory
{
    public class EntryAcceptedDtoFactory(
      IEntityService entityService,
      IPublishedContentCache publishedContentCache,
      IApiContentRouteBuilder apiContentRouteBuilder,
      IPlaceholderParsingService placeholderParsingService) : DtoFactoryBase(entityService, publishedContentCache, apiContentRouteBuilder)
    {
        private readonly IPlaceholderParsingService _placeholderParsingService = placeholderParsingService;

        public EntryAcceptedDto BuildEntryAcceptedDto(
          Form? form,
          Record record,
          Hashtable? pageElements = null,
          IDictionary<string, string?>? additionalData = null)
        {
            EntryAcceptedDto entryAcceptedDto = new();
            IPlaceholderParsingService placeholderParsingService = _placeholderParsingService;
            string str = form?.MessageOnSubmit ?? string.Empty;
            Form? form1 = form;
            Record record1 = record;
            Form? form2 = form1;
            Hashtable? pageElements1 = pageElements;
            IDictionary<string, string?>? additionalData1 = additionalData;
            entryAcceptedDto.MessageOnSubmit = placeholderParsingService.ParsePlaceHolders(str, false, record1, form2, pageElements1, additionalData1);
            entryAcceptedDto.MessageOnSubmitIsHtml = form?.MessageOnSubmitIsHtml ?? false;
            EntryAcceptedDto dto = entryAcceptedDto;
            PopulateGotoPageOnSubmit(form, dto);
            return dto;
        }
    }
}