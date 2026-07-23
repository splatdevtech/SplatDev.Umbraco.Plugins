namespace SplatDev.Payments.MercadoPago.Models
{
    public struct Address
    {
        public string StreetName { get; set; }

        public int StreetNumber { get; set; }

        public string ZipCode { get; set; }

        public string CityName { get; set; }

        public string StateName { get; set; }

        public string Floor { get; set; }

        public string Apartment { get; set; }
    }
}
