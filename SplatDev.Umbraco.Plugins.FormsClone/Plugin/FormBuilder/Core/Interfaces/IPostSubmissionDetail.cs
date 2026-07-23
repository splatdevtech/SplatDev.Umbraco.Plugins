using Umbraco.Cms.Core.Models.DeliveryApi;

namespace FormBuilder.Core.Interfaces
{
    public interface IPostSubmissionDetail
    {
        string? MessageOnSubmit { get; set; }

        bool MessageOnSubmitIsHtml { get; set; }

        Guid? GotoPageOnSubmit { get; set; }

        IApiContentRoute? GotoPageOnSubmitRoute { get; set; }
    }
}