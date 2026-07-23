using FormBuilder.Core.Interfaces;

using Umbraco.Cms.Core.Models.DeliveryApi;

namespace FormBuilder.Core.Dto
{
    public class EntryAcceptedDto : IPostSubmissionDetail
    {
        public string? MessageOnSubmit { get; set; }

        public bool MessageOnSubmitIsHtml { get; set; }

        public Guid? GotoPageOnSubmit { get; set; }

        public IApiContentRoute? GotoPageOnSubmitRoute { get; set; }
    }
}