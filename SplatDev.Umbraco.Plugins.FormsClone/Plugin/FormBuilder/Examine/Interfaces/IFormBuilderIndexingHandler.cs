using FormBuilder.Core.Persistence.Fields;

namespace FormBuilder.Examine.Interfaces
{
    public interface IFormBuilderIndexingHandler
    {
        bool Enabled { get; }

        void ReIndexForRecord(Record record);

        void DeleteRecord(Record record);
    }
}