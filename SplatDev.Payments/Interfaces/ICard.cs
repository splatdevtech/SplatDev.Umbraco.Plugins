namespace SplatDev.Payments.Interfaces
{
    using System;
    public interface ICard
    {
        string FirstSixDigits { get; set; }
        string LastFourDigits { get; set; }
        int ExpirationMonth { get; set; }
        int ExpirationYear { get; set; }
        string Status { get; set; }
        DateTime DateCreated { get; set; }
        DateTime DateLastUpdated { get; set; }
    }
}
