namespace FormBuilder.Web.Attributes
{
    /// <summary>
    /// Defines an attribute added to API methods defining details about the provided parameters.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the         /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class SwaggerParameterAttribute(string name, string description) : Attribute
    {

        /// <summary>Gets or sets the name.</summary>
        public string Name { get; private set; } = name;

        /// <summary>Gets or sets the description.</summary>
        public string Description { get; private set; } = description;
    }
}