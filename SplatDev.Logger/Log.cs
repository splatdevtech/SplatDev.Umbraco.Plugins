namespace SplatDev.Logger
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Log
    {
        public const string CLASS_NAME = nameof(Log);

        [Key]
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Display(Name = "Date Time")]
        public DateTime DateTime { get; set; }
        public LogType LogType { get; set; }
        [Display(Name = "Message")]
        public string Message { get; set; }
        [Display(Name = "Details")]
        public string Details { get; set; }

        [Display(Name = "User")]
        public string User { get; set; }

        [NotMapped]
        [Display(Name = "Log Type")]
        public string LogTypeString => LogType.TypeString();

        public override string ToString()
        {
            return $"{LogTypeString} - {Message} - {DateTime}";
        }
        public string DisplayName => this.ToString();
    }
}
