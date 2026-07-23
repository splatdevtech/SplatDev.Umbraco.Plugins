namespace SplatDev.Database
{
    using System;
    /// <summary>
    /// This attribute notifies PetaPoco that the column was altered
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AlteredColumnAttribute : Attribute
    {
        public bool AlteredColumn => true;
    }
}
