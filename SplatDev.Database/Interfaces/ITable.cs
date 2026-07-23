namespace SplatDev.Database.Interfaces
{
    public interface ITable
    {
        string TableName { get; }
        int Id { get; set; }
    }
}
