namespace SplatDev.Umbraco.Plugins.CodeFirst.Attributes
{
    using System;
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ContentNodePropertyAttributes : Attribute
    {
        public string DocumentTypePropertyAlias { get; private set; }

        public ContentNodePropertyAttributes(string DocumentTypePropertyAlias)
        {
            this.DocumentTypePropertyAlias = DocumentTypePropertyAlias;
        }
    }
}
