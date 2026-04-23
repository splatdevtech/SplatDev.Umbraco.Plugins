namespace SplatDev.Payments.MercadoPago.Models
{
    public struct TransactionDetails
    {
        
        public string AcquirerReference { get; set; }

        
        public string FinancialInstitution { get; set; }

        
        public decimal InstallmentAmount { get; set; }

        
        public decimal NetReceivedAmount { get; set; }

        
        public decimal OverpaidAmount { get; set; }

        
        public decimal TotalPaidAmount { get; set; }
    }
}
