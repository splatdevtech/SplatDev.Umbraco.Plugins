using System.Collections;

namespace FormBuilder.Core.Services.Interfaces
{
    public interface IPageService
    {
        Hashtable GetPageElements();

        Hashtable GetPageElements(int umbracoPageId);
    }
}