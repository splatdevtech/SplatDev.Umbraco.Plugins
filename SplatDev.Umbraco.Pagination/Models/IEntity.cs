namespace SplatDev.Umbraco.Pagination.Models
{
    public interface IEntity
    {
        int Id { get; set; }
        string? Url { get; set; }
    }
}
