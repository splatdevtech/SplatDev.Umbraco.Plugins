namespace SplatDev.Payments.MercadoPago.Models
{

    using System;

    public struct AdditionalInfo
    {
        public string IpAddress { get; set; }

        public PaymentItem[] Items { get; set; }

        public Payer Payer { get; set; }

        public Shipment Shipments { get; set; }

        public Barcode Barcode { get; set; }

        public DateTime RegistrationDate { get; set; }
    }
}
