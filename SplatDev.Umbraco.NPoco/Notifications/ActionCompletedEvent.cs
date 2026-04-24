using SplatDev.Umbraco.NPoco.Repositories;

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;

namespace SplatDev.Umbraco.NPoco.Notifications
{
    public class ActionCompletedEvent
    {
        public int Id { get; set; }
        public IBaseEntity? Entity { get; set; }
        public AuditType AuditType { get; set; }
        public int UserId { get; set; } = Constants.Security.SuperUserId;
        public string? Message = string.Empty;
        public bool Log { get; set; } = false;
    }

    public class UniqueActionCompletedEvent<T> where T : class
    {
        public KeyValuePair<int, int> Id { get; set; }
        public T? Entity { get; set; }
        public AuditType AuditType { get; set; }
        public int UserId { get; set; } = Constants.Security.SuperUserId;
        public string? Message = string.Empty;
        public bool Log { get; set; } = false;
    }
}
