namespace UmbracoCms.CodeFirst.Interfaces
{
    public interface IDocumentTypeCreateOrder
    {
        /// <summary>
        /// The order in which this Document Type will be created
        /// </summary>
        int CreateOrder { get; }
    }
}
