namespace SplatDev.Payments.MercadoPago.Enum
{
    public enum OrderTypes
    {
        MercadoLibre,
        MercadoPago
    }

    public static class OrderTypesHelper
    {
        public static string StringName(this OrderTypes type)
        {
            return type.ToString().ToLower();
        }
    }
}
