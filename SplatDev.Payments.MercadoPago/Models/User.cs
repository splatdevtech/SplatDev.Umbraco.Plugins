namespace SplatDev.Payments.MercadoPago.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public string SiteStatus { get; set; }
        public string Email { get; set; }
        public string SiteId { get; set; }
    }
}
