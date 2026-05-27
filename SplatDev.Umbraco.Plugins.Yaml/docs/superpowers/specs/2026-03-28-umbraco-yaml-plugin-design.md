# Umbraco YAML Plugin Design Specification

**Date:** 2026-03-28
**Status:** Approved
**Approach:** Declarative YAML + Immediate Execution (Approach A)

---

## 1. Overview

The Umbraco YAML Plugin is a declarative, Infrastructure-as-Code style plugin that enables programmmatic creation of Umbraco structures (DataTypes, DocumentTypes, Templates, and Content) from a single YAML configuration file. The plugin runs at Umbraco startup and creates or updates all defined items based on the YAML configuration.

**Purpose:** Provide a repeatable, version-controllable way to set up an Umbraco instance without manual UI interactions.

---

## 2. Scope

### In Scope
- DataTypes (built-in and custom editors)
- DocumentTypes (with properties, tabs, allowed child types)
- Templates (with inherits/master relationships)
- Content (nodes with properties, published state, sort order)
- Parent/child relationships and content hierarchies

### Out of Scope (Phase 2 or later)
- Users and permissions
- Partial Views and Macros
- Rollback/versioning/migrations
- Media files and folders
- Webhooks or event subscriptions

---

## 3. YAML Schema Structure

### 3.1 Root Structure
```yaml
umbraco:
  dataTypes: [...]
  documentTypes: [...]
  templates: [...]
  content: [...]
```

### 3.2 DataTypes
```yaml
dataTypes:
  - alias: textString           # Unique identifier
    name: Text String           # Display name
    editor: Umbraco.TextBox     # Editor alias (built-in or custom)
    config:                      # Editor-specific configuration
      maxLength: 255
```

### 3.3 DocumentTypes
```yaml
documentTypes:
  - alias: page
    name: Page
    icon: icon-document
    allowAsRoot: true           # Can be created at root level
    allowedChildTypes:          # Which DocumentTypes can be children
      - article
      - section
    tabs:
      - name: Content
        properties:
          - alias: title
            name: Title
            dataType: textString
            required: true
            description: "Page title shown in browser"
          - alias: body
            name: Body Content
            dataType: richText
            required: false
      - name: SEO
        properties:
          - alias: metaDescription
            name: Meta Description
            dataType: textString
```

### 3.4 Templates
```yaml
templates:
  - alias: page
    name: Page
    path: Views/Page.cshtml     # Relative to Views folder
    masterTemplate: null        # Can inherit from another template
```

### 3.5 Content
```yaml
content:
  - alias: home
    name: Home
    type: page                  # Reference to DocumentType alias
    published: true             # Publishing status
    sortOrder: 0
    values:
      title: Welcome
      body: "<p>Hello world</p>"
    children:                   # Nested children
      - alias: about
        name: About Us
        type: page
        published: true
        values:
          title: About
          body: "<p>About content</p>"
```

---

## 4. Plugin Architecture

### 4.1 Project Structure
```
/App_Plugins/UmbracoYaml/
  ├── Composers/
  │   └── YamlStartupComposer.cs
  ├── Services/
  │   ├── YamlParser.cs
  │   ├── DataTypeService.cs
  │   ├── DocumentTypeService.cs
  │   ├── TemplateService.cs
  │   └── ContentService.cs
  ├── Models/
  │   └── YamlModels.cs
  └── config/
      └── umbraco.yaml (example)
```

### 4.2 Key Components

**YamlParser**
- Deserializes YAML file into strongly-typed C# objects
- Validates schema and required fields
- Reports parsing errors with clear messages

**DataTypeService**
- Creates or updates DataTypes in Umbraco
- Handles editor configuration
- Prevents duplicates (checks by alias)

**DocumentTypeService**
- Creates DocumentTypes with properties and tabs
- Links properties to existing DataTypes
- Sets allowed child types and root availability
- Handles property validation rules

**TemplateService**
- Creates Templates from YAML definitions
- Links to master templates if specified
- Writes template files to disk

**ContentService**
- Creates content nodes under the root
- Establishes parent/child relationships
- Publishes content if specified in YAML
- Sets property values

**YamlStartupComposer**
- Implements IStartupHandler
- Runs on Umbraco application startup
- Orchestrates the creation flow
- Logs all operations and errors

### 4.3 Configuration
- YAML file location: `/config/umbraco.yaml` (configurable via appsettings.json)
- Plugin registers as an Umbraco v17 package
- Requires minimal dependencies (System.Yaml or similar)

---

## 5. Execution Flow

1. **Startup:** Umbraco application boots; YamlStartupComposer activates
2. **Parse:** YamlParser reads and validates `/config/umbraco.yaml`
3. **Create DataTypes:** DataTypeService creates all DataTypes in dependency order
4. **Create DocumentTypes:** DocumentTypeService creates all DocumentTypes, linking to DataTypes
5. **Create Templates:** TemplateService creates all Templates
6. **Create Content:** ContentService recursively creates content nodes with values
7. **Publish:** Content marked as `published: true` is published
8. **Log:** All successes, warnings, and errors are logged to Umbraco logs

On subsequent runs, the plugin checks if items already exist (by alias) and skips creation, or updates if necessary (future enhancement).

---

## 6. Error Handling

- **Parse errors:** Report invalid YAML syntax, missing required fields
- **Reference errors:** Report missing DataType, DocumentType, or Template references
- **Duplicate aliases:** Report and skip items with duplicate aliases
- **Creation errors:** Catch and log Umbraco service exceptions with context
- **Dependency errors:** Report missing parent DocumentTypes or Templates

All errors are logged; the plugin does NOT halt Umbraco startup but reports progress.

---

## 7. Testing Strategy

- **Unit tests:** YamlParser with valid/invalid YAML; service methods in isolation
- **Integration tests:** Full flow with test Umbraco instance
- **Fixtures:** Sample YAML files covering all schema features

---

## 8. Future Enhancements

- **Versioning & Migrations:** Track applied versions, idempotent updates
- **Rollback:** Revert to previous configurations
- **Media & Assets:** Define media folders and seed files
- **Permissions:** User roles and content permissions
- **Sync Service:** Detect and reconcile drift between YAML and Umbraco state

---

## 9. Success Criteria

- [x] Umbraco 17 fresh install with plugin
- [x] Single YAML file defines all structures
- [x] Plugin creates DataTypes, DocumentTypes, Templates, Content on startup
- [x] Content nodes are published and accessible
- [x] Proper error reporting if YAML is invalid
- [x] Clear logs showing what was created

