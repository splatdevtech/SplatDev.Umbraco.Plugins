namespace SplatDev.ScheduledTasks.Interfaces
{
    using SplatDev.ScheduledTasks.Events;

    public interface ITaskAction
    {
        void Perform(ScheduledEventArgs args);
    }
}
