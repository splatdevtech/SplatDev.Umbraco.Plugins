namespace SplatDev.ScheduledTasks.Models
{
    using SplatDev.ScheduledTasks.Enums;

    public class ScheduledTaskPayload
    {
        public string Message { get; set; }
        public ScheduledTaskType RelatedType { get; set; }
        public int DependentTaskId { get; set; }
        public ScheduledTask Task { get; set; }
        public object[] Dependencies { get; set; }
        public string[] Actions { get; set; }
    }
}
