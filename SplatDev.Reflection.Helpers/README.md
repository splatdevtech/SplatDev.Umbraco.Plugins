# SplatDev.Reflection.Helpers

Reflection and attribute helper utilities for .NET applications.
Targets `net8.0` and `net10.0`.

## Installation

```bash
dotnet add package SplatDev.Reflection.Helpers
```

## Features

- `AttributeHelper` — read and validate custom attributes at runtime
- `ReflectionHelpers` — type scanning, property access, instance creation
- Assembly scanning for plugin/extension discovery
- Simplified `Activator.CreateInstance` wrappers with caching

## Usage

### Read an attribute

```csharp
using SplatDev.Reflection.Helpers;

var attr = AttributeHelper.GetCustomAttribute<MyAttribute>(typeof(MyClass));
```

### Scan assemblies

```csharp
var types = AssemblyScanner.FindTypes<IMyInterface>();
foreach (var t in types)
{
    var instance = ReflectionHelpers.CreateInstance(t);
    // ...
}
```

### Check if attribute exists

```csharp
bool hasAttr = AttributeHelper.HasAttribute<RequiredAttribute>(typeof(MyDto));
```

## License

MIT
