namespace UmbracoCms.CodeFirst.Helpers
{
    using System.ComponentModel;
    public static class EnumHelpers
    {
        public static string GetEnumDescription<T>(this T source)
        {
            var fi = source.GetType().GetField(source.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes?.Length > 0) return attributes[0].Description;
            else return source.ToString();
        }
    }
}
