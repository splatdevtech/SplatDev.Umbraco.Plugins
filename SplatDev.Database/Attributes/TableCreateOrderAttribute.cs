namespace SplatDev.Database.Attributes
{
    using System;
    /// <summary>
    /// This attribute notifies PetaPoco the order in which this table must be created
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CreateOrderAttribute : Attribute
    {
        private readonly int _order;
        public int Order => _order;

        public CreateOrderAttribute(int order)
        {
            this._order = order;
        }
    }
}
