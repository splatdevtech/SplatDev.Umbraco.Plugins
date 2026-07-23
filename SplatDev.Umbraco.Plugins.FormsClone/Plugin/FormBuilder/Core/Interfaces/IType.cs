namespace FormBuilder.Core.Interfaces
{
    public interface IType
    {
        Guid Id { get; set; }

        string Name { get; set; }

        DateTime Created { get; set; }
    }
}