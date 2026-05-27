# Umbraco YAML Configuration Guide

This guide covers how to write YAML configuration files for the **Umbraco.Plugins.Yaml2Schema** plugin. The plugin reads a single YAML file at application startup and automatically creates DataTypes, DocumentTypes, Templates, and Content in your Umbraco installation.

---

## Table of Contents

1. [Overview](#overview)
2. [File Setup](#file-setup)
3. [File Structure](#file-structure)
4. [DataTypes](#datatypes)
5. [DocumentTypes](#documenttypes)
6. [Templates](#templates)
7. [Content](#content)
8. [Field Reference](#field-reference)
9. [Common Editors](#common-editors)
10. [Complete Example](#complete-example)
11. [Important Notes](#important-notes)

---

## Overview

The plugin follows an **Infrastructure-as-Code** approach: instead of manually building your Umbraco structure through the back-office UI, you declare everything in a YAML file. On application startup, the plugin:

1. Reads the YAML file
2. Creates **DataTypes** (the field types editors use)
3. Creates **DocumentTypes** (the content blueprints, using the DataTypes)
4. Creates **Templates** (the Razor views associated with DocumentTypes)
5. Creates **Content** nodes (the actual pages, using the DocumentTypes)

All operations are **idempotent**: items are skipped if they already exist by alias.

---

## File Setup

### Location

By default the plugin looks for the file at:

```
config/umbraco.yaml
```

This path is relative to your application root. To override it, set the following in `appsettings.json`:

```json
{
  "UmbracoYaml": {
    "ConfigPath": "config/umbraco.yaml"
  }
}
```

You may use an absolute path if needed.

### Encoding

Use **UTF-8** without a BOM. YAML is whitespace-sensitive — use **spaces only**, never tabs for indentation.

---

## File Structure

The YAML file has four top-level sections. All sections are optional, but must be declared in the root of the document — there is no `umbraco:` wrapper key.

```yaml
dataTypes:
  - ...

documentTypes:
  - ...

templates:
  - ...

content:
  - ...
```

Sections are processed in the order above. This matters because DocumentTypes reference DataTypes, and Content references DocumentTypes.

---

## DataTypes

DataTypes define the property editors available to DocumentType properties. Think of them as field type definitions.

### Syntax

```yaml
dataTypes:
  - alias: textString
    name: Text String
    editorUiAlias: Umbraco.TextBox
    config:
      maxLength: 255
```

### Fields

| Field | Required | Description |
|-------|----------|-------------|
| `alias` | Yes | Unique identifier. Referenced by DocumentType properties via `dataType`. |
| `name` | Yes | Display name shown in the Umbraco back-office. |
| `editorUiAlias` | Yes | The registered property editor alias. See [Common Editors](#common-editors). |
| `config` | No | Key-value map of editor-specific configuration options. |

### Examples

**Plain text field:**

```yaml
dataTypes:
  - alias: pageTitle
    name: Page Title
    editorUiAlias: Umbraco.TextBox
    config:
      maxLength: 100
```

**Rich text editor:**

```yaml
dataTypes:
  - alias: bodyContent
    name: Body Content
    editorUiAlias: Umbraco.TinyMCE
    config:
      toolbar: "bold,italic,underline,bullist,numlist,link"
```

**Markdown editor:**

```yaml
dataTypes:
  - alias: articleMarkdown
    name: Article Markdown
    editorUiAlias: Umbraco.MarkdownEditor
```

> **Tip:** The `alias` you give a DataType here is what you use in `dataType:` fields inside DocumentType properties. Keep aliases short and camelCase.

---

## DocumentTypes

DocumentTypes define the structure of content nodes — which properties they have, how those properties are organised into tabs, and whether content of this type can be created at the root of the content tree.

### Syntax

```yaml
documentTypes:
  - alias: page
    name: Page
    icon: icon-document
    allowAsRoot: true
    allowedChildTypes:
      - page
      - article
    tabs:
      - name: Content
        properties:
          - alias: title
            name: Title
            dataType: textString
            required: true
            description: The main heading of the page

          - alias: bodyContent
            name: Body Content
            dataType: richText
            required: false
```

### Fields

| Field | Required | Default | Description |
|-------|----------|---------|-------------|
| `alias` | Yes | — | Unique identifier. Used when referencing this type in `content` nodes and `allowedChildTypes`. |
| `name` | Yes | — | Display name in the back-office. |
| `icon` | No | — | CSS icon class from the Umbraco icon library (e.g. `icon-document`, `icon-article`). |
| `allowAsRoot` | No | `true` | Whether content of this type can be created at the content tree root. |
| `allowedChildTypes` | No | `[]` | List of DocumentType aliases that can be created as children of this type. |
| `tabs` | No | `[]` | List of tabs that group properties together. |

### Tab Fields

| Field | Required | Description |
|-------|----------|-------------|
| `name` | Yes | Tab display name (e.g. `Content`, `Settings`, `SEO`). |
| `properties` | No | List of properties inside this tab. |

### Property Fields

| Field | Required | Default | Description |
|-------|----------|---------|-------------|
| `alias` | Yes | — | Unique identifier within the DocumentType. Used as the property key when accessing content values in templates. |
| `name` | Yes | — | Label shown to editors in the back-office. |
| `dataType` | Yes | — | The `alias` of a DataType defined in the `dataTypes` section. |
| `required` | No | `false` | If `true`, editors must fill in this field before saving. |
| `description` | No | — | Help text shown below the field in the back-office. |

### Example with multiple tabs

```yaml
documentTypes:
  - alias: article
    name: Article
    icon: icon-article
    allowAsRoot: false
    allowedChildTypes: []
    tabs:
      - name: Content
        properties:
          - alias: headline
            name: Headline
            dataType: textString
            required: true

          - alias: body
            name: Body
            dataType: richText
            required: true

      - name: Metadata
        properties:
          - alias: metaTitle
            name: Meta Title
            dataType: textString
            description: Overrides the page title in search results

          - alias: metaDescription
            name: Meta Description
            dataType: textString
            description: Short description for search engine results

      - name: Authoring
        properties:
          - alias: author
            name: Author
            dataType: textString
            required: true

          - alias: publishDate
            name: Publish Date
            dataType: textString
            required: true
```

---

## Templates

Templates define the Razor view files associated with your content. The plugin creates the template record in Umbraco; the corresponding `.cshtml` file must exist on disk at the path specified.

### Syntax

```yaml
templates:
  - alias: page
    name: Page
    path: Page.cshtml
    masterTemplate: null
```

### Fields

| Field | Required | Description |
|-------|----------|-------------|
| `alias` | Yes | Unique identifier for the template. |
| `name` | Yes | Display name shown in the back-office. |
| `path` | Yes | Path to the `.cshtml` file, relative to the `Views/` folder. |
| `masterTemplate` | No | The `alias` of another template to use as the layout parent. Set to `null` for a root/master template. |

### Template inheritance

Use `masterTemplate` to create a layout hierarchy. A child template sets its `masterTemplate` to the alias of the parent:

```yaml
templates:
  - alias: master
    name: Master
    path: Master.cshtml
    masterTemplate: null

  - alias: page
    name: Page
    path: Page.cshtml
    masterTemplate: master

  - alias: article
    name: Article
    path: Article.cshtml
    masterTemplate: master
```

The corresponding `Page.cshtml` would set `Layout = "Master"` in its Razor `@{ }` block.

> **Note:** The plugin creates the template *record* in Umbraco. The `.cshtml` file itself must exist at the path you specify, or Umbraco will be unable to render content using that template.

---

## Content

The `content` section seeds actual content nodes. Nodes can be nested to arbitrary depth using the `children` key.

### Syntax

```yaml
content:
  - alias: home
    name: Home
    documentType: page
    isPublished: true
    sortOrder: 0
    properties:
      title: "Welcome"
      bodyContent: "<p>Hello world</p>"
    children:
      - alias: about
        name: About Us
        documentType: page
        isPublished: true
        properties:
          title: "About Us"
          bodyContent: "<p>Who we are</p>"
```

### Fields

| Field | Required | Default | Description |
|-------|----------|---------|-------------|
| `alias` | Yes | — | Unique identifier for this content node. |
| `name` | Yes | — | The node name shown in the content tree. |
| `documentType` | Yes | — | The `alias` of the DocumentType this node is an instance of. |
| `isPublished` | No | `false` | Whether to publish the node on creation. Unpublished nodes are saved as drafts. |
| `sortOrder` | No | `0` | Sort position among siblings (zero-based). |
| `properties` | No | `{}` | Key-value map of property values. Keys are the property `alias` values from the DocumentType. |
| `children` | No | `[]` | Nested child content nodes. Follows the same structure recursively. |

### Property values

Values in `properties` map directly to DocumentType property aliases. String values should be quoted if they contain special YAML characters (`:`, `#`, `{`, etc.).

```yaml
properties:
  title: "Our Services"
  metaDescription: "Discover what we offer"
  bodyContent: "<p>We provide <strong>quality services</strong>.</p>"
  publishDate: "2026-01-15"
  author: Jane Doe
```

For rich text properties, include HTML as a quoted string. YAML multi-line strings can also be used for readability:

```yaml
properties:
  bodyContent: |
    <p>First paragraph of content.</p>
    <p>Second paragraph of content.</p>
```

### Deep nesting example

```yaml
content:
  - alias: home
    name: Home
    documentType: page
    isPublished: true
    properties:
      title: "Home"
    children:

      - alias: blog
        name: Blog
        documentType: page
        isPublished: true
        properties:
          title: "Blog"
        children:

          - alias: first-post
            name: First Post
            documentType: article
            isPublished: true
            properties:
              headline: "My First Post"
              body: "<p>Content here.</p>"
              author: "John Smith"
              publishDate: "2026-03-01"
```

---

## Field Reference

Quick-reference summary of all field names and their types.

### DataType

```
alias          string  (required) unique identifier
name           string  (required) display name
editorUiAlias  string  (required) editor plugin alias
config         map     (optional) editor configuration
```

### DocumentType

```
alias              string   (required) unique identifier
name               string   (required) display name
icon               string   (optional) CSS icon class
allowAsRoot        boolean  (optional, default: true)
allowedChildTypes  list     (optional) list of DocumentType aliases
tabs               list     (optional) list of Tab objects
```

### Tab

```
name        string  (required) tab label
properties  list    (optional) list of Property objects
```

### Property

```
alias        string   (required) unique identifier within DocumentType
name         string   (required) label shown to editors
dataType     string   (required) alias of a DataType
required     boolean  (optional, default: false)
description  string   (optional) help text for editors
```

### Template

```
alias           string  (required) unique identifier
name            string  (required) display name
path            string  (required) path to .cshtml relative to Views/
masterTemplate  string  (optional) alias of parent template, or null
```

### Content

```
alias         string   (required) unique identifier
name          string   (required) node display name
documentType  string   (required) alias of a DocumentType
isPublished   boolean  (optional, default: false)
sortOrder     integer  (optional, default: 0)
properties    map      (optional) property alias → value pairs
children      list     (optional) nested Content objects
```

---

## Common Editors

Use these values for `editorUiAlias` in your DataType definitions.

| `editorUiAlias` | Description |
|-----------------|-------------|
| `Umbraco.TextBox` | Single-line text input |
| `Umbraco.TextArea` | Multi-line plain text |
| `Umbraco.RichText` | Rich text editor |
| `Umbraco.TinyMCE` | TinyMCE rich HTML editor |
| `Umbraco.MarkdownEditor` | Markdown input |
| `Umbraco.Integer` | Whole number field |
| `Umbraco.Decimal` | Decimal number field |
| `Umbraco.TrueFalse` | Boolean toggle |
| `Umbraco.Date` | Date picker |
| `Umbraco.DateTime` | Date and time picker |
| `Umbraco.MediaPicker3` | Media picker |
| `Umbraco.ContentPicker` | Content node picker |
| `Umbraco.MultipleTextstring` | List of text strings |
| `Umbraco.Tags` | Tag input |
| `Umbraco.DropDown.Flexible` | Dropdown select list |
| `Umbraco.CheckBoxList` | Multi-select checkbox list |
| `Umbraco.ColorPicker` | Color picker |
| `Umbraco.Slider` | Numeric range slider |

> Any custom property editor registered with your Umbraco installation can also be used — supply its alias as the `editorUiAlias` value.

---

## Complete Example

A complete, working YAML file demonstrating all four sections:

```yaml
# Umbraco YAML Configuration
# Defines DataTypes, DocumentTypes, Templates, and seed Content

dataTypes:
  - alias: textString
    name: Text String
    editorUiAlias: Umbraco.TextBox

  - alias: richText
    name: Rich Text
    editorUiAlias: Umbraco.TinyMCE

  - alias: markdown
    name: Markdown
    editorUiAlias: Umbraco.MarkdownEditor

documentTypes:
  - alias: page
    name: Page
    icon: icon-document
    allowAsRoot: true
    allowedChildTypes:
      - page
      - article
    tabs:
      - name: Content
        properties:
          - alias: title
            name: Title
            dataType: textString
            required: true
          - alias: bodyContent
            name: Body Content
            dataType: richText
      - name: SEO
        properties:
          - alias: metaTitle
            name: Meta Title
            dataType: textString
          - alias: metaDescription
            name: Meta Description
            dataType: textString
            description: Shown in search engine results

  - alias: article
    name: Article
    icon: icon-article
    allowAsRoot: false
    tabs:
      - name: Content
        properties:
          - alias: headline
            name: Headline
            dataType: textString
            required: true
          - alias: body
            name: Body
            dataType: richText
            required: true
          - alias: summary
            name: Summary
            dataType: textString
      - name: Metadata
        properties:
          - alias: author
            name: Author
            dataType: textString
            required: true
          - alias: publishDate
            name: Publish Date
            dataType: textString
            required: true

templates:
  - alias: page
    name: Page
    path: Page.cshtml
    masterTemplate: null

  - alias: article
    name: Article
    path: Article.cshtml
    masterTemplate: page

content:
  - alias: home
    name: Home
    documentType: page
    isPublished: true
    sortOrder: 0
    properties:
      title: "Welcome"
      bodyContent: "<p>Welcome to our website.</p>"
      metaTitle: "Home"
      metaDescription: "Our company home page"
    children:

      - alias: about
        name: About Us
        documentType: page
        isPublished: true
        sortOrder: 0
        properties:
          title: "About Us"
          bodyContent: "<p>Learn about our company.</p>"

      - alias: blog
        name: Blog
        documentType: page
        isPublished: true
        sortOrder: 1
        properties:
          title: "Blog"
          bodyContent: "<p>Our latest articles.</p>"
        children:

          - alias: first-article
            name: Getting Started
            documentType: article
            isPublished: true
            sortOrder: 0
            properties:
              headline: "Getting Started with Umbraco YAML"
              body: "<p>This article explains how to use the YAML plugin.</p>"
              summary: "A beginner's guide to YAML configuration"
              author: "Jane Doe"
              publishDate: "2026-03-01"
```

---

## Important Notes

### Aliases must be unique within their section

The plugin detects duplicates within a single YAML file and skips subsequent items with the same alias. Aliases are **case-sensitive**.

### Processing is ordered

DataTypes are created before DocumentTypes. DocumentTypes are created before Content. If a property references a DataType that doesn't exist (or failed to create), the property is skipped with a warning in the logs.

### Idempotency

If an item with a given alias already exists in Umbraco, the plugin skips creation. It does **not** update or overwrite existing items. This means the YAML file is safe to have in place across restarts.

### `required` vs `mandatory`

The correct field name for marking a property as required is `required` (boolean). Some example files use `mandatory` — this is silently ignored by the parser.

### Template files must exist separately

The plugin creates the template *record* in Umbraco's database, but it does not create the `.cshtml` file. You must create the Razor view file manually at the path specified in `path:`.

### Rich text content

When seeding rich text properties via `properties`, provide the value as an HTML string. Use YAML block scalars (`|`) for multi-line content:

```yaml
properties:
  bodyContent: |
    <h2>Section Heading</h2>
    <p>Paragraph one.</p>
    <p>Paragraph two.</p>
```

### Logging

All creation activity and any warnings (missing DataTypes, skipped duplicates) are written to the standard Umbraco log. Check the log if content does not appear as expected after startup.
