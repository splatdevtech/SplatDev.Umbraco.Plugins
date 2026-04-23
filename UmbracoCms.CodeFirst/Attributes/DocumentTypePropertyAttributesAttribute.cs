namespace UmbracoCms.CodeFirst.Attributes
{
    using System;
    using System.Collections.Generic;

    using UmbracoCms.Plugins;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DocumentTypePropertyAttributesAttribute : Attribute
    {
        /// <summary>
        /// Name for the Property
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Alias of the Property
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// Type of the property i.e. radio button, text string, etc.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Tab in which the property will be
        /// </summary>
        public string Tab { get; }

        /// <summary>
        /// The description of the property
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The order in which it will be displayed in the tab
        /// </summary>
        public int? SortOrder { get; set; }

        /// <summary>
        /// Prevalues for the Document Type
        /// </summary>
        public List<string> PreValues { get; set; }

        /// <summary>
        /// Configures the The Property Attribute
        /// </summary>
        /// <param name="Name">Name for the Property</param>
        /// <param name="Alias"></param>
        /// <param name="Type"></param>
        /// <param name="Description"></param>
        /// <param name="Tab"></param>
        /// <param name="SortOrder"></param>
        public DocumentTypePropertyAttributesAttribute(string Name, string Alias, string Type = Default.DataTypes.Alias.Custom, string Description = "", string Tab = "", int SortOrder = 0, string[] PreValues = null)
        {
            this.Name = Name;
            this.Alias = Alias;
            this.Type = Type;
            this.Description = Description;
            this.Tab = Tab;
            this.SortOrder = SortOrder;

            if (PreValues != null)
            {
                this.PreValues = new List<string>();
                for (int i = 0; i < PreValues.Length; i++)
                    this.PreValues.Add(PreValues[i]);
            }
        }
    }
}
