namespace SplatDev.Payments.MercadoPago.Enum
{
    public enum PaymentMethodTypes
    {
        BolBradesco,
        Pix,
        Master,
        Visa,
        Amex,
        Elo,
        Cabal,
        Debcabal,
        Debmaster,
        Hipercard,
        Debvisa,
        Debelo,
        Meliplace,
        Pec,
        Account_Money,
    }
    public static class PaymentMethodTypesHelper
    {
        public static string StringName(this PaymentMethodTypes type)
        {
            return type.ToString().ToLower();
        }
    }
}
