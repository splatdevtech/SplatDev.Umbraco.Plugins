using FormBuilder.Core.Models;

namespace FormBuilder.Core.Storage.Interfaces
{
    public interface IPreValueTextFileStorage
    {
        void DeleteTextFile(string filePath);

        List<Prevalue> GetTextFilePreValues(string filePath);

        void SaveValuesIntoFile(List<string> values, string filePath);

        string GenerateFilePath(string filename, Guid preValueId);

        void SaveTextFile(Stream fileContentStream, string filename, Guid preValueId);
    }
}