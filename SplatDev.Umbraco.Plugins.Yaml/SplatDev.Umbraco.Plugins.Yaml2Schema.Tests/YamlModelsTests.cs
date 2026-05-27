using Xunit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Tests
{
    public class YamlModelsTests
    {
        private static IDeserializer BuildDeserializer() =>
            new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

        // ── YamlRoot / DataTypes ───────────────────────────────────────────────

        [Fact]
        public void YamlRoot_ShouldDeserializeFromValidYaml()
        {
            var yaml = @"
umbraco:
  dataTypes:
    - alias: textString
      name: Text String
      editorUiAlias: Umbraco.TextBox
      config:
        maxLength: 255
";
            var root = BuildDeserializer().Deserialize<YamlRoot>(yaml);

            Assert.NotNull(root);
            Assert.NotNull(root.Umbraco);
            Assert.Single(root.Umbraco.DataTypes);
            Assert.Equal("textString", root.Umbraco.DataTypes[0].Alias);
        }

        [Fact]
        public void YamlDataType_ShouldDeserializeRemoveFlag()
        {
            var yaml = @"
umbraco:
  dataTypes:
    - alias: oldType
      name: Old Type
      editorUiAlias: Umbraco.TextBox
      remove: true
";
            var root = BuildDeserializer().Deserialize<YamlRoot>(yaml);

            Assert.True(root.Umbraco.DataTypes[0].Remove);
            Assert.False(root.Umbraco.DataTypes[0].Update);
        }

        [Fact]
        public void YamlDataType_ShouldDeserializeUpdateFlag()
        {
            var yaml = @"
umbraco:
  dataTypes:
    - alias: myType
      name: My Type
      editorUiAlias: Umbraco.TextBox
      update: true
";
            var root = BuildDeserializer().Deserialize<YamlRoot>(yaml);

            Assert.True(root.Umbraco.DataTypes[0].Update);
            Assert.False(root.Umbraco.DataTypes[0].Remove);
        }

        // ── DocumentType ──────────────────────────────────────────────────────

        [Fact]
        public void DocumentType_ShouldAllowProperties()
        {
            var yaml = @"
umbraco:
  documentTypes:
    - alias: page
      name: Page
      icon: icon-document
      allowAsRoot: true
      tabs:
        - name: Content
          properties:
            - alias: title
              name: Title
              dataType: textString
              required: true
";
            var root = BuildDeserializer().Deserialize<YamlRoot>(yaml);

            Assert.Single(root.Umbraco.DocumentTypes);
            var docType = root.Umbraco.DocumentTypes[0];
            Assert.Equal("page", docType.Alias);
            Assert.Single(docType.Tabs);
            Assert.Single(docType.Tabs[0].Properties);
        }

        [Fact]
        public void YamlDocumentType_ShouldDeserializeRemoveAndUpdateFlags()
        {
            var yaml = @"
umbraco:
  documentTypes:
    - alias: old
      name: Old
      remove: true
    - alias: existing
      name: Existing
      update: true
";
            var root = BuildDeserializer().Deserialize<YamlRoot>(yaml);

            Assert.True(root.Umbraco.DocumentTypes[0].Remove);
            Assert.True(root.Umbraco.DocumentTypes[1].Update);
        }

        // ── YamlTemplate ──────────────────────────────────────────────────────

        [Fact]
        public void YamlTemplate_ShouldDeserializeScriptsAndStylesheets()
        {
            var yaml = @"
umbraco:
  templates:
    - alias: masterPage
      name: Master Page
      stylesheets:
        - css/site.css
      scripts:
        - js/app.js
";
            var root = BuildDeserializer().Deserialize<YamlRoot>(yaml);

            var tpl = root.Umbraco.Templates[0];
            Assert.Single(tpl.Stylesheets);
            Assert.Equal("css/site.css", tpl.Stylesheets[0]);
            Assert.Single(tpl.Scripts);
            Assert.Equal("js/app.js", tpl.Scripts[0]);
        }

        [Fact]
        public void YamlTemplate_ShouldDeserializeRemoveAndUpdateFlags()
        {
            var yaml = @"
umbraco:
  templates:
    - alias: old
      name: Old
      remove: true
    - alias: live
      name: Live
      update: true
";
            var root = BuildDeserializer().Deserialize<YamlRoot>(yaml);

            Assert.True(root.Umbraco.Templates[0].Remove);
            Assert.True(root.Umbraco.Templates[1].Update);
        }

        // ── YamlScript / YamlStylesheet ───────────────────────────────────────

        [Fact]
        public void YamlScript_ShouldDeserializeAllFields()
        {
            var yaml = @"
umbraco:
  scripts:
    - alias: siteJs
      name: Site JS
      path: js/site.js
      content: console.log('hello');
      update: true
    - alias: legacyJs
      name: Legacy JS
      path: js/legacy.js
      remove: true
";
            var root = BuildDeserializer().Deserialize<YamlRoot>(yaml);

            Assert.Equal(2, root.Umbraco.Scripts.Count);
            var s0 = root.Umbraco.Scripts[0];
            Assert.Equal("siteJs", s0.Alias);
            Assert.Equal("js/site.js", s0.Path);
            Assert.Equal("console.log('hello');", s0.Content);
            Assert.True(s0.Update);
            Assert.False(s0.Remove);

            var s1 = root.Umbraco.Scripts[1];
            Assert.True(s1.Remove);
            Assert.False(s1.Update);
        }

        [Fact]
        public void YamlStylesheet_ShouldDeserializeAllFields()
        {
            var yaml = @"
umbraco:
  stylesheets:
    - alias: siteCss
      name: Site CSS
      path: css/site.css
      content: 'body { margin: 0; }'
      update: true
    - alias: oldCss
      name: Old CSS
      path: css/old.css
      remove: true
";
            var root = BuildDeserializer().Deserialize<YamlRoot>(yaml);

            Assert.Equal(2, root.Umbraco.Stylesheets.Count);
            var c0 = root.Umbraco.Stylesheets[0];
            Assert.Equal("siteCss", c0.Alias);
            Assert.Equal("css/site.css", c0.Path);
            Assert.True(c0.Update);
            Assert.False(c0.Remove);

            Assert.True(root.Umbraco.Stylesheets[1].Remove);
        }

        // ── YamlContent flags ─────────────────────────────────────────────────

        [Fact]
        public void YamlContent_ShouldDeserializeRemoveAndUpdateFlags()
        {
            var yaml = @"
umbraco:
  content:
    - alias: old
      name: Old Page
      documentType: page
      remove: true
    - alias: home
      name: Home
      documentType: page
      update: true
";
            var root = BuildDeserializer().Deserialize<YamlRoot>(yaml);

            Assert.True(root.Umbraco.Content[0].Remove);
            Assert.True(root.Umbraco.Content[1].Update);
        }

        // ── YamlDataType.valueType ─────────────────────────────────────────────

        [Fact]
        public void YamlDataType_ShouldDeserializeValueType()
        {
            var yaml = @"
umbraco:
  dataTypes:
    - alias: customEditor
      name: Custom Frontend Editor
      editorUiAlias: My.CustomEditor
      valueType: NTEXT
";
            var root = BuildDeserializer().Deserialize<YamlRoot>(yaml);

            Assert.Equal("NTEXT", root.Umbraco.DataTypes[0].ValueType);
        }

        [Fact]
        public void YamlDataType_ValueTypeShouldBeNullByDefault()
        {
            var yaml = @"
umbraco:
  dataTypes:
    - alias: textBox
      name: Text Box
      editorUiAlias: Umbraco.TextBox
";
            var root = BuildDeserializer().Deserialize<YamlRoot>(yaml);

            Assert.Null(root.Umbraco.DataTypes[0].ValueType);
        }

        // ── YamlMedia.folder ──────────────────────────────────────────────────

        [Fact]
        public void YamlMedia_ShouldDeserializeFolder()
        {
            var yaml = @"
umbraco:
  media:
    - name: Brand Logo
      mediaType: Image
      url: https://example.com/logo.png
      folder: Images/Brands
";
            var root = BuildDeserializer().Deserialize<YamlRoot>(yaml);

            Assert.Equal("Images/Brands", root.Umbraco.Media[0].Folder);
        }

        [Fact]
        public void YamlMedia_FolderShouldBeNullByDefault()
        {
            var yaml = @"
umbraco:
  media:
    - name: Photo
      mediaType: Image
";
            var root = BuildDeserializer().Deserialize<YamlRoot>(yaml);

            Assert.Null(root.Umbraco.Media[0].Folder);
        }

        // ── YamlPackage ───────────────────────────────────────────────────────

        [Fact]
        public void YamlPackage_ShouldDeserializeAllFields()
        {
            var yaml = @"
umbraco:
  packages:
    - id: Examine
      version: 4.0.0
      required: true
      assemblyName: Examine
    - id: SomeOptional.Package
      required: false
";
            var root = BuildDeserializer().Deserialize<YamlRoot>(yaml);

            Assert.Equal(2, root.Umbraco.Packages.Count);

            var first = root.Umbraco.Packages[0];
            Assert.Equal("Examine", first.Id);
            Assert.Equal("4.0.0", first.Version);
            Assert.True(first.Required);
            Assert.Equal("Examine", first.AssemblyName);

            var second = root.Umbraco.Packages[1];
            Assert.Equal("SomeOptional.Package", second.Id);
            Assert.False(second.Required);
        }

        [Fact]
        public void YamlPackage_RequiredShouldDefaultToFalse()
        {
            var yaml = @"
umbraco:
  packages:
    - id: SomePackage
";
            var root = BuildDeserializer().Deserialize<YamlRoot>(yaml);

            Assert.False(root.Umbraco.Packages[0].Required);
        }

        // ── YamlPropertyEditor ────────────────────────────────────────────────

        [Fact]
        public void YamlPropertyEditor_ShouldDeserializeAllFields()
        {
            var yaml = @"
umbraco:
  propertyEditors:
    - alias: Acme.Rating
      name: Star Rating
      icon: icon-star
      group: special
      uiAlias: Acme.Rating.Ui
      folderName: acme-rating
      jsPath: /App_Plugins/acme-rating/index.js
      jsContent: |
        customElements.define('acme-rating', class extends HTMLElement {});
      update: true
    - alias: Acme.Old
      name: Old Editor
      remove: true
";
            var root = BuildDeserializer().Deserialize<YamlRoot>(yaml);

            Assert.Equal(2, root.Umbraco.PropertyEditors.Count);

            var first = root.Umbraco.PropertyEditors[0];
            Assert.Equal("Acme.Rating", first.Alias);
            Assert.Equal("Star Rating", first.Name);
            Assert.Equal("icon-star", first.Icon);
            Assert.Equal("special", first.Group);
            Assert.Equal("Acme.Rating.Ui", first.UiAlias);
            Assert.Equal("acme-rating", first.FolderName);
            Assert.Equal("/App_Plugins/acme-rating/index.js", first.JsPath);
            Assert.False(string.IsNullOrWhiteSpace(first.JsContent));
            Assert.True(first.Update);
            Assert.False(first.Remove);

            var second = root.Umbraco.PropertyEditors[1];
            Assert.True(second.Remove);
            Assert.False(second.Update);
        }

        [Fact]
        public void YamlPropertyEditor_FlagsShouldDefaultToFalse()
        {
            var yaml = @"
umbraco:
  propertyEditors:
    - alias: My.Editor
      name: My Editor
";
            var root = BuildDeserializer().Deserialize<YamlRoot>(yaml);

            Assert.False(root.Umbraco.PropertyEditors[0].Remove);
            Assert.False(root.Umbraco.PropertyEditors[0].Update);
        }

        // ── UmbracoConfig default collections ─────────────────────────────────

        [Fact]
        public void UmbracoConfig_PackagesAndPropertyEditorsShouldDefaultToEmptyList()
        {
            var yaml = @"
umbraco:
  dataTypes: []
";
            var root = BuildDeserializer().Deserialize<YamlRoot>(yaml);

            Assert.NotNull(root.Umbraco.Packages);
            Assert.Empty(root.Umbraco.Packages);
            Assert.NotNull(root.Umbraco.PropertyEditors);
            Assert.Empty(root.Umbraco.PropertyEditors);
        }
    }
}
