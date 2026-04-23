namespace SplatDev.Payments.MercadoPago.Enum
{
    public enum PayerTypes
    {
        Customer,
        Registered,
        Guest
    }

    public static class PayerTypesHelper
    {
        public static string StringName(this PayerTypes type)
        {
            return type.ToString().ToLower();
        }
    }
}
