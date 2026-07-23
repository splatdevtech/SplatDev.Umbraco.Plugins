
// Type: Umbraco.Forms.Web.JQueryUIDatePickerHelper
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Threading;


#nullable enable
namespace Umbraco.Forms.Web
{
  public static class JQueryUIDatePickerHelper
  {
    public static string ConvertDateFormat(this HtmlHelper html) => html.ConvertDateFormat(Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern);

    public static string ConvertDateFormat(this HtmlHelper html, string format)
    {
      string str1 = format.Replace("dddd", "DD").Replace("ddd", "D");
      string str2 = !str1.Contains("MMMM") ? (!str1.Contains("MMM") ? (!str1.Contains("MM") ? str1.Replace("M", "m") : str1.Replace("MM", "mm")) : str1.Replace("MMM", "M")) : str1.Replace("MMMM", "MM");
      return str2.Contains("yyyy") ? str2.Replace("yyyy", "yy") : str2.Replace("yy", "y");
    }
  }
}
