namespace SplatDev.Payments.MercadoPago.Models
{

    using System;

    using SplatDev.Payments.Interfaces;
    public class CreditCard : ICard, IPaymentMethod
    {
        
        public string Id { get; set; }

        
        public bool LiveMode { get; set; }

        
        public bool LuhnValidation { get; set; }

        
        public bool RequireEsc { get; set; }

        
        public int SecurityCodeLength { get; set; }

        
        public string CardNumber { get; set; }

        
        public int ExpirationMonth { get; set; }

        
        public int ExpirationYear { get; set; }

        
        public CardHolder Cardholder { get; set; }

        
        public string SecurityCode { get; set; }

        
        public string FirstSixDigits { get; set; }

        
        public string LastFourDigits { get; set; }

        
        public string Status { get; set; }

        
        public DateTime DateCreated { get; set; }

        
        public DateTime DateLastUpdated { get; set; }

        
        public string PaymentMethodId { get; set; }

        
        public int Installments { get; set; }

        public override string ToString()
        {
            return $"Token: {Id} - Last 4: {LastFourDigits} ({Status})";
        }
    }
}
