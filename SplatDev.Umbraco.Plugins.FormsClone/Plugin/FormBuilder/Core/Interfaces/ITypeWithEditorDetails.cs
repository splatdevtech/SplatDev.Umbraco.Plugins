namespace FormBuilder.Core.Interfaces
{
    public interface ITypeWithEditorDetails : IType
    {
        int? CreatedBy { get; set; }

        string? CreatedByName { get; set; }

        DateTime Updated { get; set; }

        int? UpdatedBy { get; set; }

        string? UpdatedByName { get; set; }
    }
}