namespace SplatDev.Umbraco.Plugins.WordsApi.Services
{
    public interface IWordsApiService
    {
        Task<bool> IsNoun(string word, string apiKey);
    }
}