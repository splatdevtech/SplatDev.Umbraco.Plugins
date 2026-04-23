namespace SplatDev.Database.Attributes
{
    using System;
    /// <summary>
    /// This attribute notifies PetaPoco that the column needs to be nvarchar(max)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NvarcharMaxAttribute : Attribute {
        public bool Value => true;
    }
}
