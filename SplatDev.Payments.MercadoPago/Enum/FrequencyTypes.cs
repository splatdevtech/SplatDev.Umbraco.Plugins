namespace SplatDev.Payments.MercadoPago.Enum
{
    public enum FrequencyTypes
    {
        Days,
        Months
    }

    public static class FrequencyTypesHelper
    {
        public static string StringName(this FrequencyTypes type)
        {
            return type.ToString().ToLower();
        }
    }
}
