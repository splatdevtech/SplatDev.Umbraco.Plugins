namespace SplatDev.Security.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("IpHistory")]
    public class IpHistory
    {
        [Key, Index]
        public int Id { get; set; }

        public DateTime OccurredOn { get; set; }
        public EventType Event { get; set; }
        public string Ip { get; set; }
        public string Url { get; set; }
        public string Exception { get; set; }
    }

    public enum EventType
    {
        Error,
        LoginFailure
    }
}
