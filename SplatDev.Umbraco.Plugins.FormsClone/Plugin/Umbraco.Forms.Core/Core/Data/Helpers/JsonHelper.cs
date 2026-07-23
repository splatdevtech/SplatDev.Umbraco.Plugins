
// Type: Umbraco.Forms.Core.Data.Helpers.JsonHelper
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Text;


#nullable enable
namespace Umbraco.Forms.Core.Data.Helpers
{
  public class JsonHelper
  {
    public static string EscapeStringValue(string? value)
    {
      if (string.IsNullOrEmpty(value))
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder(value.Length);
      foreach (char ch in value)
      {
        switch (ch)
        {
          case '\'':
            stringBuilder.AppendFormat("{0}{1}", (object) '\\', (object) '\'');
            break;
          case '/':
            stringBuilder.AppendFormat("{0}{1}", (object) '\\', (object) '/');
            break;
          case '\\':
            stringBuilder.AppendFormat("{0}{0}", (object) '\\');
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
