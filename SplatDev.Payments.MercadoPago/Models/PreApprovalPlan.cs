namespace SplatDev.Payments.MercadoPago.Models
{

    using System;
    public class PreApprovalPlan
    {
        
        public string Id { get; set; }

        
        public string Reason { get; set; }

        
        public AutoRecurring AutoRecurring { get; set; }

        
        public string BackUrl { get; set; }

        
        public long CollectorId { get; set; }

        
        public string Status { get; set; }

        
        public DateTime DateCreated { get; set; }

        
        public DateTime LastModified { get; set; }

        
        public string InitPoint { get; set; }

        
        public string SandboxInitPoint { get; set; }
    }
}
