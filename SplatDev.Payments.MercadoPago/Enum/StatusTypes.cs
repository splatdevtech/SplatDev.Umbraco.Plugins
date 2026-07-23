namespace SplatDev.Payments.MercadoPago.Enum
{
    public enum StatusTypes
    {
        Authorized,
        Pending,
        Cancelled,
        Paused
    }
    public static class StatusTypesHelper
    {
        public static string StringName(this StatusTypes type)
        {
            return type.ToString().ToLower();
        }
    }
}
