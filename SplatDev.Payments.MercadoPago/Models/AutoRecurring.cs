namespace SplatDev.Payments.MercadoPago.Models
{
    using System;
    using Newtonsoft.Json;
    public struct AutoRecurring
    {
        public int Frequency { get; set; }

        public string FrequencyType { get; set; }

        public decimal TransactionAmount { get; set; }

        public string CurrencyId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Repetitions { get; set; }

        public FreeTrial FreeTrial { get; set; }

        public int BillingDay { get; set; }

        public bool BillingDayProportional { get; set; }

    }
}
