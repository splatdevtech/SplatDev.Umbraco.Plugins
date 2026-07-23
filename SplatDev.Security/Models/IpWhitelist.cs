namespace SplatDev.Security.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("IpWhitelist")]
    public class IpWhitelist
    {
        [Key]
        public int Id { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedOn { get; set; }
        public string Ip { get; set; }
    }
}
