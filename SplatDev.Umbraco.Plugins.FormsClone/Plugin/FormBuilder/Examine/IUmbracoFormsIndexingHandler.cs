using FormBuilder.Core.Persistence.Fields;

namespace FormBuilder.Examine
{
    public interface IFormBuildersIndexingHandler
    {
        bool Enabled { get; }

        void ReIndexForRecord(Record record);

        void DeleteRecord(Record record);
    }
}