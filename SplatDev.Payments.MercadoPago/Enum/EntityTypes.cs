namespace SplatDev.Payments.MercadoPago.Enum
{
    public enum EntityTypes
    {
        Individual,
        Association
    }

    public static class IndividualTypesHelper
    {
        public static string StringName(this EntityTypes type)
        {
            return type.ToString().ToLower();
        }
    }
}
