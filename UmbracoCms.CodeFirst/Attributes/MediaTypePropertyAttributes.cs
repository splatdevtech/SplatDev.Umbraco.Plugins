namespace UmbracoCms.CodeFirst.Attributes
{
    using System;

    using UmbracoCms.Plugins;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class MediaPropertyAttributesAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Alias { get; private set; }
        public string Type { get; private set; }
        public string Tab { get; private set; }
        public string Description { get; private set; }

        public MediaPropertyAttributesAttribute(string Name, string Alias, string Type = Default.DataTypes.Alias.Custom, string Description = "", string Tab = "")
        {
            this.Name = Name;
            this.Alias = Alias;
            this.Type = Type;
            this.Description = Description;
            this.Tab = Tab;
        }
    }
}
