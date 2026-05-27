# Completed Tasks

## Features

- Add support for JavaScript and CSS static assets (`scripts` / `stylesheets` YAML sections, `StaticAssetCreator`, template tag injection)
- Add `[REMOVE]` flag support across all YAML sections (dataTypes, documentTypes, templates, content, scripts, stylesheets, mediaTypes, media, languages, dictionaryItems, members, users)
- Add `[UPDATE]` flag support across all YAML sections (upsert semantics per entity type)
- Add support for Block List and Grid Block config — `config:` dictionary on `dataTypes` entries is now applied to `DataType.Configuration`
- Add support for dictionary items (`dictionaryItems` section, `DictionaryCreator` service)
- Add support for multilanguage (`languages` section, `LanguageCreator` service)
- Add support for including Razor code in template section (`content:` field on templates, overrides auto-generated scaffold)
- Add support for adding images to Media by downloading them from a web URL (`media` section with `url:` field, `MediaCreator` service using `IHttpClientFactory`)
- Add support for creating media types (`mediaTypes` section, `MediaTypeCreator` service)
- Add support for creating members (`members` section, `MemberCreator` service)
- Add support for creating users (`users` section, `UserCreator` service)

## Tests

- `StaticAssetCreatorTests` — 18 tests (CREATE, REMOVE, UPDATE, dedup, null guards, path normalisation)
- `YamlModelsTests` — extended with deserialization tests for all flags and new model types
- `TemplateCreatorTests` — REMOVE, UPDATE, create-when-update-missing, HTML injection tests
- `DataTypeCreatorTests` — REMOVE, UPDATE, null list guard tests
- `DocumentTypeCreatorTests` — refactored to class-level fixtures; REMOVE, UPDATE, create-when-update-missing tests
- `ContentCreatorTests` — refactored to shared `Build()` helper; REMOVE, UPDATE, create-when-update-missing tests
- `YamlStartupComposerTests` — `StaticAssetCreator` registration assertion added
- `WebProjectConfigTests` — 13 smoke tests against the live web project `config/umbraco.yml`
- `IntegrationTests` — updated for new model counts, new section assertions (languages, dictionaryItems, mediaTypes, media, members, users, Razor content in templates)
- `YamlParserTests` — updated template count assertion

## Documentation

- `CHANGELOG.md` — full changelog covering v1.0.0, v1.0.1, v1.0.2, and v1.0.3 (unreleased)
- `fixtures/sample.yml` — updated with examples of all new sections
