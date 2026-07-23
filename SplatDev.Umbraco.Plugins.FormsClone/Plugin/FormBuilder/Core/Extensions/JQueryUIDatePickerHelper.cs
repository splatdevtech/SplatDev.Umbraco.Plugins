using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FormBuilder.Core.Extensions
{
    /// <summary>
    /// Provides extension methods on     /// </summary>
    public static class JQueryUIDatePickerHelper
    {
        /// <summary>
        /// Converts the .net supported date format current culture format into JQuery Datepicker format.
        /// </summary>
        /// <param name="html">HtmlHelper object.</param>
        /// <returns>Format string that supported in JQuery Datepicker.</returns>
        public static string ConvertDateFormat(this HtmlHelper html) => html.ConvertDateFormat(Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern);

        /// <summary>
        /// Converts the .net supported date format current culture format into JQuery Datepicker format.
        /// </summary>
        /// <param name="html">HtmlHelper object.</param>
        /// <param name="format">Date format supported by .NET.</param>
        /// <returns>Format string that supported in JQuery Datepicker.</returns>
        public static string ConvertDateFormat(this HtmlHelper html, string format)
        {
            ArgumentNullException.ThrowIfNull(html);

            string str1 = format.Replace("dddd", "DD").Replace("ddd", "D");
            string str2 = !str1.Contains("MMMM") ? !str1.Contains("MMM") ? !str1.Contains("MM") ? str1.Replace("M", "m") : str1.Replace("MM", "mm") : str1.Replace("MMM", "M") : str1.Replace("MMMM", "MM");
            return str2.Contains("yyyy") ? str2.Replace("yyyy", "yy") : str2.Replace("yy", "y");
        }
    }
}