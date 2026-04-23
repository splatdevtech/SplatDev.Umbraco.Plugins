using System.Text;

namespace FormBuilder.Core.Helpers
{
    public class JsonHelper
    {
        public static string EscapeStringValue(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            StringBuilder stringBuilder = new(value.Length);
            foreach (char ch in value)
            {
                switch (ch)
                {
                    case '\'':
                        stringBuilder.AppendFormat("{0}{1}", '\\', '\'');
                        break;

                    case '/':
                        stringBuilder.AppendFormat("{0}{1}", '\\', '/');
                        break;

                    case '\\':
                        stringBuilder.AppendFormat("{0}{0}", '\\');
                        break;

                    default:
                        stringBuilder.Append(ch);
                        break;
                }
            }
            return stringBuilder.ToString();
        }
    }
}