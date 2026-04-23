using System.Text;

namespace SplatDev.Umbraco._2FA.Extensions
{
    public static partial class TwoFactorAuthorizationExtensions
    {
        public static string GeneratePassword(int length = 12, bool nonAlphanumeric = true, bool digit = true, bool lowercase = true, bool uppercase = true)
        {
            StringBuilder password = new();
            Random random = new();

            while (password.Length < length)
            {
                char c = (char)random.Next(32, 126);

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
                password.Append((char)random.Next(33, 48));
            if (digit)
                password.Append((char)random.Next(48, 58));
            if (lowercase)
                password.Append((char)random.Next(97, 123));
            if (uppercase)
                password.Append((char)random.Next(65, 91));

            return password.ToString();
        }

        public static string GeneratePin()
        {
            Random random = new();

            // Generate a 6-digit PIN
            string pin = "";
            for (int i = 0; i < 6; i++)
            {
                pin += random.Next(0, 10).ToString();
            }

            return pin;
        }
    }
}
