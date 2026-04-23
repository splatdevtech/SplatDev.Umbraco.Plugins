namespace SplatDev.Content
{
    public static class QRHelpers
    {
        public static string GenerateQR(int size = 150, string data = "")
        {
            return string.Format("https://api.qrserver.com/v1/create-qr-code/?size={0}x{0}&data={1}", size, data);
        }
    }
}
