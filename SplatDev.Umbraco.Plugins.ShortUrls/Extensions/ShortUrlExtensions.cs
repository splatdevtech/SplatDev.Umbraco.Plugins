namespace SplatDev.Umbraco.Plugins.ShortUrls.Extensions
{
    internal class ShortUrlExtensions
    {
        public static string GenerateRandomUrl(int size = 7)
        {
            return RandomString(size);
        }
        /// <summary>
        /// Generates a random string
        /// </summary>
        /// <param name="length"></param>
        /// <returns>Random String</returns>
        /// <![CDATA[Taken from http://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings-in-c]]>
        private static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYZ0123456789_-";
            var random = new Random();
            return new string([.. Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)])]);
        }
    }
}
