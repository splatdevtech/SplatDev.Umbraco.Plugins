namespace FormBuilder.Core.Models
{
    public class RegenerateFormStructureIdsResult(Dictionary<Guid, Guid> fieldIdMapping)
    {
        public Dictionary<Guid, Guid> FieldIdMapping { get; } = fieldIdMapping;
    }
}