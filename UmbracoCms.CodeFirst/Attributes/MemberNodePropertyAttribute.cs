namespace UmbracoCms.CodeFirst.Attributes
{
    using System;
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class MemberNodePropertyAttribute : Attribute
    {
        public string MemberTypePropertyAlias { get; private set; }

        public MemberNodePropertyAttribute(string MemberTypePropertyAlias)
        {
            this.MemberTypePropertyAlias = MemberTypePropertyAlias;
        }
    }
}
