
// Type: Umbraco.Forms.Core.Models.DeliveryApi.EntryAcceptedDtoFactory
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections;
using System.Collections.Generic;
using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Models.DeliveryApi
{
  public class EntryAcceptedDtoFactory : DtoFactoryBase
  {
    private readonly IPlaceholderParsingService _placeholderParsingService;

    public EntryAcceptedDtoFactory(
      IEntityService entityService,
      IPublishedContentCache publishedContentCache,
      IApiContentRouteBuilder apiContentRouteBuilder,
      IPlaceholderParsingService placeholderParsingService)
      : base(entityService, publishedContentCache, apiContentRouteBuilder)
    {
      this._placeholderParsingService = placeholderParsingService;
    }

    public EntryAcceptedDto BuildEntryAcceptedDto(
      Form form,
      Record record,
      Hashtable? pageElements = null,
      IDictionary<string, string?>? additionalData = null)
    {
      EntryAcceptedDto entryAcceptedDto = new EntryAcceptedDto();
      IPlaceholderParsingService placeholderParsingService = this._placeholderParsingService;
      string str = form.MessageOnSubmit ?? string.Empty;
      Form form1 = form;
      Record record1 = record;
      Form form2 = form1;
      Hashtable pageElements1 = pageElements;
      IDictionary<string, string> additionalData1 = additionalData;
      entryAcceptedDto.MessageOnSubmit = placeholderParsingService.ParsePlaceHolders(str, false, record1, form2, pageElements1, additionalData1);
      entryAcceptedDto.MessageOnSubmitIsHtml = form.MessageOnSubmitIsHtml;
      EntryAcceptedDto dto = entryAcceptedDto;
      this.PopulateGotoPageOnSubmit(form, (IPostSubmissionDetail) dto);
      return dto;
    }
  }
}
