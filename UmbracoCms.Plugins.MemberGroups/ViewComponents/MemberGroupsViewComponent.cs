using Microsoft.AspNetCore.Mvc;
using UmbracoCms.Plugins.MemberGroups.Services;

namespace UmbracoCms.Plugins.MemberGroups.ViewComponents
{
    public class MemberGroupsViewComponent : ViewComponent
    {
        private readonly IMemberGroupsService _service;

        public MemberGroupsViewComponent(IMemberGroupsService service)
        {
            _service = service;
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            var groups = _service.GetMemberGroups();
            var types = _service.GetMemberTypes();

            ViewBag.Groups = groups;
            ViewBag.Types = types;

            return Task.FromResult<IViewComponentResult>(View());
        }
    }
}
