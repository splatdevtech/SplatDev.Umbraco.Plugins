namespace FormBuilder.Extension.Attributes
{
    /// <summary>
    /// Attribute to mark a property as a workflow setting for the Form Builder plugin.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SettingAttribute"/> class.
    /// </remarks>
    /// <param name="name">The display name of the setting.</param>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SettingAttribute(string name) : Attribute
    {
        /// <summary>
        /// The display name of the setting.
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// The description/help text for the setting.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The view/editor type for the setting (e.g., "TextField", "TextArea", "Dropdown").
        /// </summary>
        public string? View { get; set; }
    }
}
