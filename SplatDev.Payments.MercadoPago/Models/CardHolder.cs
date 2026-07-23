namespace SplatDev.Payments.MercadoPago.Models
{
    public struct CardHolder
    {
        
        public Identification Identification { get; set; }

        
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }


}
