namespace SplatDev.ScheduledTasks.Models
{
    using SplatDev.ScheduledTasks.Enums;

    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ScheduledTask
    {
        public const string CLASS_NAME = "Scheduled Task";

        [Key]
        public int Id { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        [Display(Name = "Repeat")]
        public bool Repeat { get; set; }

        [Display(Name = "Repeat every x hours")]
        public int RepeatEveryXHours { get; set; }

        [Display(Name = "Repeat every x minutes")]
        public int RepeatEveryXMinutes { get; set; }

        [Display(Name = "Start on")]
        public DateTime StartOn { get; set; }

        [Display(Name = "End on (optional)")]
        public DateTime? EndOn { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Task Type")]
        public ScheduledTaskType TaskType { get; set; }

        [Display(Name = "Perfom once done")]
        public int TaskId { get; set; }

        [NotMapped]
        public ScheduledTask Task { get; set; }

        [Display(Name = "Criteria Type")]
        public ScheduledTaskCriteriaType CriteriaType { get; set; }

        [Display(Name = "Criteria")]
        public string Criteria { get; set; }

        public string DisplayName => this.ToString();

        [Display(Name = "Invoke", Description = "Assembly to Invoke when Ellapsed")]
        public string ClassToInvoke { get; set; }

        public int UnitId { get; set; }

        public bool StopOnError { get; set; }

        [NotMapped]
        public string UnitName { get; set; }

        public DateTime? LastRunOnUtc { get; set; }
        public DateTime? NextRun { get; set; }
        public override string ToString()
        {
            return $"{Name} [Start: {StartOn} | Every: {RepeatEveryXHours} hours, {RepeatEveryXMinutes} minutes]";
        }
    }
}
