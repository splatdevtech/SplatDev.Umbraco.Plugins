using System.ComponentModel;

namespace FormBuilder.Core.Configuration
{
    public class ScheduledRecordDeletionSettings
    {
        internal const bool StaticEnabled = false;
        internal const string StaticPeriod = "1.00:00:00";

        [DefaultValue(false)]
        public bool Enabled { get; set; }

        public string FirstRunTime { get; set; } = string.Empty;

        [DefaultValue("1.00:00:00")]
        public TimeSpan Period { get; set; } = TimeSpan.Parse("1.00:00:00");
    }
}