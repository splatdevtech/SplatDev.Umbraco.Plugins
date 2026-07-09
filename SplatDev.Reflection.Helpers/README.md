# SplatDev.Reflection.Helpers

Reflection and attribute inspection utilities for .NET applications. Provides helpers for type discovery, property enumeration, custom attribute reading, and dynamic instantiation — used across the SplatDev plugin ecosystem.

## Install

```bash
dotnet add package SplatDev.System.Reflection.Helpers
```

Targets `net8.0` and `net10.0`. No external dependencies beyond `System.Reflection`.

## What's implemented

### Attribute inspection

Read custom attributes from types, properties, and attribute instances.

```csharp
using System.Reflection;

// Check if a type has an attribute
bool hasAttr = typeof(MyModel).HasAttribute<MyCustomAttribute>();

// Read a specific attribute value
var attr = typeof(MyModel).GetAttribute<MyCustomAttribute>();
string name = attr.Value<string>("Name");

// Flatten all attributes into a dictionary
var all = typeof(MyModel).GetClassAttributes();

// Read attributes from a property via lambda
var props = ReflectionHelper.GetPropertyAttributes<MyModel>(() => instance.PropertyName);
```

| Method | Targets | Returns |
|--------|---------|---------|
| `HasAttribute<T>()` | `Type`, `PropertyInfo` | `bool` |
| `GetAttribute<T>()` | `Type`, `PropertyInfo` | `Attribute` or typed `T` |
| `GetClassAttributes()` | `Type` | `Dictionary<string, object>` (flat keyed by `"AttributeName.PropertyName"`) |
| `GetPropertyAttributes<T>()` | Expression | `Dictionary<string, object>` |
| `Value<T>()` | `Attribute` | Named attribute property value cast to `T` |

### Type and assembly discovery

```csharp
// Get all types matching a base type or interface
var controllers = ReflectionHelper.GetTypes<ControllerBase>();

// Load an assembly by name
var asm = ReflectionHelper.GetAssembly("MyPlugin");

// Find a type within an assembly
var type = asm.GetType<MyService>();

// Get all properties (including nested hierarchies)
var props = typeof(MyModel).GetAllProperties();

// For interfaces, walks the full interface graph recursively
var ifaceProps = typeof(IMyInterface).GetAllProperties();
```

| Method | Description |
|--------|-------------|
| `GetTypes<T>(assemblyName)` | Returns all types in an assembly whose `Type == typeof(T)`. Static. |
| `GetAssembly(name)` | Loads an assembly by name. Defaults to executing assembly. Static. |
| `GetType<T>()` | Retrieves a `Type` object matching `T` from an assembly |
| `GetAllProperties()` | Enumerates all properties of a type including nested hierarchy |

### Dynamic instantiation

```csharp
// Create an instance from a Type
object instance = type.GetInstance();
MyService svc = type.GetInstance<MyService>();

// Default construction via generic
var svc = ReflectionHelper.GetTypeInstance<MyService>();

// Locate and instantiate from an assembly
var svc = assembly.GetAssemblyType<MyService>();
```

### Property value access

```csharp
// Get a PropertyInfo by name
var prop = instance.GetProperty("Name");

// Check/read values
bool hasVal = prop.HasValue();
object val = prop.Value();
string name = prop.Value<string>();

// Read collection/dictionary properties
var items = prop.ValueCollection<string>();
var dict = prop.ValueCollection<string, int>();
```

## Dependencies

Netstandard — no external NuGet dependencies.

---

Copyright SplatDev
