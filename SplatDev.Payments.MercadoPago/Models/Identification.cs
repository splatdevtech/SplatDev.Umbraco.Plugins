namespace SplatDev.Payments.MercadoPago.Models
{
    public struct Identification
    {
        
        public string Number { get; set; }

        
        public string Type { get; set; }

        
        public string Id { get; set; }

        
        public string Name { get; set; }

        
        public int MinLength { get; set; }

        
        public int MaxLength { get; set; }

        public override string ToString()
        {
            return $"{Number} ({Type})";
        }
    }
}
