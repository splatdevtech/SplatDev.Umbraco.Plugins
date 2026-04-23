using FormBuilder.Core.Models;

namespace FormBuilder.Core.Interfaces
{
    public interface IConditioned
    {
        FieldCondition? Condition { get; set; }
    }
}