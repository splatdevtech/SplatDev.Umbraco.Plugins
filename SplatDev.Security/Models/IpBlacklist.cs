namespace SplatDev.Security.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("IpBlacklist")]
    public class IpBlacklist
    {
        [Key, Index]
        public int Id { get; set; }

        public DateTime BannedOn { get; set; }
        public string BannedBy { get; set; }
        public DateTime ReleaseOn { get; set; }
        public string Ip { get; set; }
    }
}
