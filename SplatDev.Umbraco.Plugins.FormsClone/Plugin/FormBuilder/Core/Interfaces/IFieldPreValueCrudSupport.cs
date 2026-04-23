using FormBuilder.Core.Models;

namespace FormBuilder.Core.Interfaces
{
    public interface IFieldPrevalueCrudSupport
    {
        Prevalue InsertValue(Prevalue preValue, Field field, Form form);

        Prevalue UpdateValue(Prevalue preValue, Field field, Form form);

        bool DeleteValue(Prevalue preValue, Field field, Form form);

        void SortValues(Guid fieldId, List<Prevalue> values, Field field, Form form);
    }
}