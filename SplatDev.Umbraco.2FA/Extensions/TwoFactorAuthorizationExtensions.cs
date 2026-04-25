using System.Security.Cryptography;
using System.Text;

namespace SplatDev.Umbraco._2FA.Extensions
{
    public static partial class TwoFactorAuthorizationExtensions
    {
        public static string GeneratePassword(int length = 12, bool nonAlphanumeric = true, bool digit = true, bool lowercase = true, bool uppercase = true)
        {
            StringBuilder password = new();

            while (password.Length < length)
            {
                char c = (char)RandomNumberGenerator.GetInt32(32, 126);

                password.Append(c);

                if (char.IsDigit(c))
                    digit = false;
                else if (char.IsLower(c))
                    lowercase = false;
                else if (char.IsUpper(c))
                    uppercase = false;
                else if (!char.IsLetterOrDigit(c))
                    nonAlphanumeric = false;
            }

            if (nonAlphanumeric)
                password.Append((char)RandomNumberGenerator.GetInt32(33, 48));
            if (digit)
                password.Append((char)RandomNumberGenerator.GetInt32(48, 58));
            if (lowercase)
                password.Append((char)RandomNumberGenerator.GetInt32(97, 123));
            if (uppercase)
                password.Append((char)RandomNumberGenerator.GetInt32(65, 91));

            return password.ToString();
        }

        public static string GeneratePin()
        {
            StringBuilder pin = new();
            for (int i = 0; i < 6; i++)
            {
                pin.Append(RandomNumberGenerator.GetInt32(0, 10));
            }
            return pin.ToString();
        }
    }
}
