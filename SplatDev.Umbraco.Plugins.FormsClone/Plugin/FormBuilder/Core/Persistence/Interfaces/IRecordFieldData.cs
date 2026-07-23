namespace FormBuilder.Core.Persistence.Interfaces
{
    public interface IRecordFieldData
    {
        int Id { get; set; }

        Guid Key { get; set; }
    }
}