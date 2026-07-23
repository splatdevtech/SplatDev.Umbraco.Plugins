namespace SplatDev.Payments.MercadoPago.Enum
{
    public enum BarcodeTypes
    {
        UCCEAN128,
        Code128C,
        Code39
    }

    public static class BarcodeTypesHelper
    {
        public static string StringName(this BarcodeTypes type)
        {
            switch (type)
            {
                case BarcodeTypes.UCCEAN128:
                    return "UCC/EAN 128";
                default:
                    return type.ToString();
            }
        }
    }
}
