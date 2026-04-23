# UmbracoCms.CodeFirst [DEPRECATED]

> WARNING: This package is deprecated. Use [SplatDev.Umbraco.Plugins.Yaml2Schema](https://github.com/SplatDev-Ltda/umbraco-yaml) instead.

Code-first content type definitions for Umbraco 13 and 17.

This was an old implementation that defined Umbraco content types via C# attributes (DocumentTypeAttribute, MediaTypeAttribute, etc.). It has been superseded by Umbraco-Yaml which provides a more flexible declarative YAML-based approach.

## Migration to Umbraco-Yaml

Instead of defining your schema via C# attributes, define it in a YAML file and use SplatDev.Umbraco.Plugins.Yaml2Schema to import it.

## Supported Versions
- Umbraco 13 (net8.0)
- Umbraco 17 (net10.0)
