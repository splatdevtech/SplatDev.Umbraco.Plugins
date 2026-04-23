namespace UmbracoCms.CodeFirst.Attributes
{
    using System;
    using System.Collections.Generic;

    using UmbracoCms.Plugins;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class MemberPropertyAttributesAttribute : Attribute
    {
        /// <summary>
        /// Configures the The Property Attribute
        /// </summary>
        /// <param name="Name">Name for the Property</param>
        /// <param name="Alias"></param>
        /// <param name="Type"></param>
        /// <param name="Description"></param>
        /// <param name="Tab"></param>
        /// <param name="SortOrder"></param>
        /// <param name="PreValues"></param>
        /// <param name="MemberCanEdit"></param>
        /// <param name="ShowOnMemberProfile"></param>
        /// <param name="IsSensitive"></param>
        /// <param name="Mandatory"></param>
        /// <param name="ValidationRegExp"></param>
        public MemberPropertyAttributesAttribute(string Name, string Alias, string Type = Default.DataTypes.Alias.Custom, string Description = "", string Tab = "", int SortOrder = 0, string[] PreValues = null, bool MemberCanEdit = false, bool ShowOnMemberProfile = false, bool IsSensitive = false, bool Mandatory = false, string ValidationRegExp = "")
        {
            this.Name = Name;
            this.Alias = Alias;
            this.Type = Type;
            this.Description = Description;
            this.Tab = Tab;
            this.SortOrder = SortOrder;
            this.MemberCanEdit = MemberCanEdit;
            this.ShowOnMemberProfile = ShowOnMemberProfile;
            this.IsSensitive = IsSensitive;
            this.Mandatory = Mandatory;
            this.ValidationRegExp = ValidationRegExp;

            if (PreValues != null)
            {
                this.PreValues = new List<string>();
                for (int i = 0; i < PreValues.Length; i++)
                    this.PreValues.Add(PreValues[i]);
            }
        }

        /// <summary>
        /// Alias of the Property
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// The description of the property
        /// </summary>
        public string Description { get; }

        public bool IsSensitive { get; }

        public bool Mandatory { get; }

        public bool MemberCanEdit { get; }

        /// <summary>
        /// Name for the Property
        /// </summary>
        public string Name { get; }
        public List<string> PreValues { get; set; }

        public bool ShowOnMemberProfile { get; }

        /// <summary>
        /// The order in which it will be displayed in the tab
        /// </summary>
        public int? SortOrder { get; set; }

        /// <summary>
        /// Tab in which the property will be
        /// </summary>
        public string Tab { get; }

        /// <summary>
        /// Type of the property i.e. radio button, text string, etc.
        /// </summary>
        public string Type { get; }
        public string ValidationRegExp { get; }
    }
}
