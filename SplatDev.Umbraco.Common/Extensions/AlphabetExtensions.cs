namespace SplatDev.Umbraco.Common.Extensions
{
    public class AlphabetExtensions
    {
        public static char[] Alphabet()
        {
            char[] alphabet = new char[26];
            var index = 0;
            for (int i = 65; i <= 90; i++)
            {
                alphabet[index++] = (char)i;
            }
            return alphabet;
        }
    }
}
