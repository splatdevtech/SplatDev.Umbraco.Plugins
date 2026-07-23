namespace SplatDev.Payments.Helpers
{
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;

    public static class SecureConnectionHelpers
    {
        public static void OverrideCertificateValidation()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(RemoteCertValidate);
        }

        public static bool RemoteCertValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policy)
        {
            return true;
        }
    }
}
