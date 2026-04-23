namespace SplatDev.Payments.MercadoPago.Models
{
    using SplatDev.Payments.Interfaces;
    public struct Payer : IPayer
    {

        public string FirstName { get; set; }


        public string LastName { get; set; }


        public Address Address { get; set; }


        public string Email { get; set; }


        public int Id { get; set; }


        public Identification Identification { get; set; }


        public Phone Phone { get; set; }


        public string Type { get; set; }


        public string EntityType { get; set; }
    }
}
