namespace SplatDev.Umbraco.Plugins.CodeFirst.Attributes
{
    using System;
    /// <summary>
    /// This Attribute tells the engine that this annotation marks a Document Type 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MediaTypeAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Alias { get; private set; }
        public bool AllowAtRoot { get; private set; }
        public string Description { get; private set; }
        public string Icon { get; private set; }

        public MediaTypeAttribute(string Name, string Alias, bool AllowAtRoot,string Description = "", string Icon = "")
        {
            this.Name = Name;
            this.Alias = Alias;
            this.AllowAtRoot = AllowAtRoot;
            this.Description = Description;
            this.Icon = Icon;
        }
    }
}
