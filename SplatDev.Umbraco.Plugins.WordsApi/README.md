# SplatDev.Umbraco.Plugins.WordsApi

Umbraco plugin that provides dictionary word lookup via the WordsAPI service on RapidAPI. The primary use case is checking whether a word is a noun — useful for content validation, text analysis, or generating noun-based content identifiers.

## Install

```bash
dotnet add package SplatDev.Umbraco.Plugins.WordsApi
```

Multi-targets `net8.0` (Umbraco 13) and `net10.0` (Umbraco 17). Published to nuget.org.

## What's implemented

### `IWordsApiService`

```csharp
public interface IWordsApiService
{
    Task<bool> IsNoun(string word, string apiKey);
}
```

Returns `true` if the word has at least one definition whose part of speech is `"noun"`. Returns `false` on any error or if no noun definition is found.

### `WordsApiService`

Calls the WordsAPI on RapidAPI:

```
GET https://wordsapiv1.p.rapidapi.com/words/{word}/definitions
Headers:
  X-RapidAPI-Key: {apiKey}
  X-RapidAPI-Host: wordsapiv1.p.rapidapi.com
```

Deserializes the response into `WordDefinitions` and checks for noun entries.

### Models

```csharp
public class WordDefinitions
{
    public string Word { get; set; }
    public DefinitionDetails[] Definitions { get; set; }

    public class DefinitionDetails
    {
        public string Definition { get; set; }
        public string PartOfSpeech { get; set; }
    }
}
```

## Usage

```csharp
var service = serviceProvider.GetRequiredService<IWordsApiService>();

bool isNoun = await service.IsNoun("developer", "your-rapidapi-key");

if (isNoun)
    Console.WriteLine("'developer' is a noun.");
```

## Configuration

No appsettings keys. The API key is passed as a method parameter to `IsNoun()`.

### Getting a RapidAPI key

1. Sign up at [rapidapi.com](https://rapidapi.com)
2. Subscribe to the [WordsAPI](https://rapidapi.com/dpventures/api/wordsapi) service
3. Copy your `X-RapidAPI-Key` from the RapidAPI dashboard

## DI Registration

Automatic via `WordsApiComposer : IComposer`. Umbraco's `TypeFinder` discovers and runs the composer at startup. No `.AddWordsApi()` call needed.

| Service | Lifetime |
|---------|----------|
| `IWordsApiService` → `WordsApiService` | Scoped |

## Dependencies

| Package | Purpose |
|---------|---------|
| `Umbraco.Cms.Core` | Composers, DI |

---

**SplatDev.Umbraco.Plugins.WordsApi** — part of the [SplatDev.Umbraco.Plugins](https://github.com/SplatDev-Ltda/SplatDev.Umbraco.Plugins) suite. Licensed under MIT. © SplatDev Ltda.
