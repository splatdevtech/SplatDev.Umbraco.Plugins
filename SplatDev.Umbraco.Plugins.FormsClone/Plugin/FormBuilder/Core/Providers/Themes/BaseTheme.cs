namespace FormBuilder.Core.Providers.Themes
{
    /// <summary>Defines the default theme and files.</summary>
    public abstract class BaseTheme
    {
        /// <summary>Gets the format for retrieving theme file file paths.</summary>
        protected const string FilePathFormat = "{0}/{1}/{2}.cshtml";
    }
}