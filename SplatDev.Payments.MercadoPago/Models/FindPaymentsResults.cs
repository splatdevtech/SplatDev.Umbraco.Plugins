namespace SplatDev.Payments.MercadoPago.Models
{
    public class FindPaymentsResults
    {
        
        public Paging Paging { get; set; }

        
        public Result[] Results { get; set; }
    }
}
