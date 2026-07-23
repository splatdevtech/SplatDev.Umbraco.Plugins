namespace SplatDev.Umbraco.Plugins.CodeFirst.Attributes
{
    using System;
    /// <summary>
    /// This Attribute tells the engine that this annotation marks a Document Type 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoMapperAttribute : Attribute
    {
        public string Name { get; private set; }
        public string ForMemberAlias { get; private set; }

        public AutoMapperAttribute(string Name, string ForMemberAlias)
        {
            this.Name = Name;
            this.ForMemberAlias = ForMemberAlias;
        }
    }
}
