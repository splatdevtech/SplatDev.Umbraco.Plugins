namespace SplatDev.Payments.MercadoPago.Models
{
    public struct PaymentItem
    {
        
        public string Id { get; set; }

        
        public string Title { get; set; }

        
        public string Description { get; set; }

        
        public string PictureUrl { get; set; }

        
        public string CategoryId { get; set; }

        
        public int Quantity { get; set; }

        
        public decimal UnitPrice { get; set; }
    }
}
