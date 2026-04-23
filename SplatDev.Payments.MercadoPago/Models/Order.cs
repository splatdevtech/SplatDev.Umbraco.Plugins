namespace SplatDev.Payments.MercadoPago.Models
{
    using SplatDev.Payments.Interfaces;
    public struct Order:IOrder
    {
        
        public string Type { get; set; }

        
        public int Id { get; set; }
    }
}
