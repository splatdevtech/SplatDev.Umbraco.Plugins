using SplatDev.Umbraco.Plugins.WordsApi.Models;

using System.Text.Json;

namespace SplatDev.Umbraco.Plugins.WordsApi.Services
{
    public class WordsApiService : IWordsApiService
    {
        public async Task<bool> IsNoun(string word, string apiKey = "cb16878eb2mshe5e3df9fa630b10p1234afjsn91e7c82fc7c9")
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://wordsapiv1.p.rapidapi.com/words/{word}/definitions"),
                Headers =
                {
                    { "X-RapidAPI-Key", apiKey },
                    { "X-RapidAPI-Host", "wordsapiv1.p.rapidapi.com" },
                },
            };
            try
            {
                using var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                WordDefinitions definitions = new();

                if (!string.IsNullOrEmpty(responseBody))
                    definitions = JsonSerializer.Deserialize<WordDefinitions>(responseBody)!;

                if (definitions is null) return false;
                return definitions.Definitions.Where(x => x.PartOfSpeech == "noun").Any();
            }
            catch
            {
                return false;
            }
        }
    }
}
