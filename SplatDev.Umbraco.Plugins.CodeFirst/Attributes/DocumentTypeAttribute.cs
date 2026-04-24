namespace SplatDev.Umbraco.Plugins.CodeFirst.Attributes
{
    using System;
    /// <summary>
    /// This Attribute tells the engine that this annotation marks a Document Type 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DocumentTypeAttribute : Attribute
    {
        public bool IsDocumentType => true;
        public string Name { get; private set; }
        public string Alias { get; private set; }
        public bool AllowAtRoot { get; private set; }
        public string Description { get; private set; }
        public string Icon { get; private set; }
        public string IconColor { get; private set; }
        public string Container { get; private set; }
        public string ContainerAlias { get; private set; }
        public int? ContainerLevel { get; private set; }
        public int SortOrder { get; private set; }
        public int CreatorId { get; private set; }

        public DocumentTypeAttribute(string Name, string Alias, bool AllowAtRoot, int SortOrder, int CreatorId = 0, string Description = "", string Icon = "", string IconColor = "", string Container = "", string ContainerAlias = "", int ContainerLevel = -1)
        {
            this.Name = Name;
            this.Alias = Alias;
            this.AllowAtRoot = AllowAtRoot;
            this.Description = Description;
            this.Icon = Icon;
            this.IconColor = IconColor;
            this.Container = Container;
            this.ContainerAlias = ContainerAlias;
            this.ContainerLevel = ContainerLevel;
            this.SortOrder = SortOrder;
            this.CreatorId = CreatorId;

            var containerArray = this.Container.Split(' ');
            containerArray[0] = containerArray[0].ToLower();
            this.ContainerAlias = string.Join("", containerArray);
        }
    }
}
