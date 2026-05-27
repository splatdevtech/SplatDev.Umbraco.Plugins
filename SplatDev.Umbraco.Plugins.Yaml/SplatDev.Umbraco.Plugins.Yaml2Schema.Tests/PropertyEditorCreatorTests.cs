using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Services;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Tests
{
    public class PropertyEditorCreatorTests : IDisposable
    {
        private readonly string _wwwroot;
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly PropertyEditorCreator _creator;

        public PropertyEditorCreatorTests()
        {
            _wwwroot = Path.Combine(Path.GetTempPath(), "pe-tests-" + Path.GetRandomFileName());
            Directory.CreateDirectory(_wwwroot);

            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockEnv.Setup(x => x.WebRootPath).Returns(_wwwroot);

            _creator = new PropertyEditorCreator(_mockEnv.Object);
        }

        public void Dispose()
        {
            if (Directory.Exists(_wwwroot))
                Directory.Delete(_wwwroot, recursive: true);
        }

        // ── Guard clauses ────────────────────────────────────────────────────

        [Fact]
        public void CreatePropertyEditors_ShouldNotThrowOnNullList()
        {
            var ex = Record.Exception(() => _creator.CreatePropertyEditors(null!));
            Assert.Null(ex);
        }

        [Fact]
        public void CreatePropertyEditors_ShouldNotThrowOnEmptyList()
        {
            var ex = Record.Exception(() => _creator.CreatePropertyEditors(new List<YamlPropertyEditor>()));
            Assert.Null(ex);
        }

        [Fact]
        public void Constructor_ShouldThrowWhenWebHostEnvironmentIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new PropertyEditorCreator(null!));
        }

        // ── Manifest creation ─────────────────────────────────────────────────

        [Fact]
        public void CreatePropertyEditors_ShouldWriteManifest()
        {
            _creator.CreatePropertyEditors(new List<YamlPropertyEditor>
            {
                new YamlPropertyEditor
                {
                    Alias = "My.CustomEditor",
                    Name = "My Custom Editor",
                    Icon = "icon-star",
                    Group = "special"
                }
            });

            var manifestPath = Path.Combine(_wwwroot, "App_Plugins", "my-customeditor", "umbraco-package.json");
            Assert.True(File.Exists(manifestPath));
        }

        [Fact]
        public void CreatePropertyEditors_ShouldWriteValidJson()
        {
            _creator.CreatePropertyEditors(new List<YamlPropertyEditor>
            {
                new YamlPropertyEditor
                {
                    Alias = "Test.Editor",
                    Name = "Test Editor"
                }
            });

            var manifestPath = Path.Combine(_wwwroot, "App_Plugins", "test-editor", "umbraco-package.json");
            var json = File.ReadAllText(manifestPath);
            var doc = JsonDocument.Parse(json);

            Assert.Equal("Test Editor", doc.RootElement.GetProperty("name").GetString());
            Assert.Equal("1.0.0", doc.RootElement.GetProperty("version").GetString());

            var extensions = doc.RootElement.GetProperty("extensions");
            Assert.Equal(JsonValueKind.Array, extensions.ValueKind);
            Assert.Equal(2, extensions.GetArrayLength());
        }

        [Fact]
        public void CreatePropertyEditors_ManifestShouldContainPropertyEditorSchemaExtension()
        {
            _creator.CreatePropertyEditors(new List<YamlPropertyEditor>
            {
                new YamlPropertyEditor { Alias = "Acme.Rating", Name = "Rating" }
            });

            var manifestPath = Path.Combine(_wwwroot, "App_Plugins", "acme-rating", "umbraco-package.json");
            var json = File.ReadAllText(manifestPath);
            var doc = JsonDocument.Parse(json);

            var extensions = doc.RootElement.GetProperty("extensions");
            bool foundSchema = false;
            foreach (var ext in extensions.EnumerateArray())
            {
                if (ext.GetProperty("type").GetString() == "propertyEditorSchema"
                    && ext.GetProperty("alias").GetString() == "Acme.Rating")
                {
                    foundSchema = true;
                    break;
                }
            }
            Assert.True(foundSchema, "Should have a propertyEditorSchema extension with correct alias");
        }

        [Fact]
        public void CreatePropertyEditors_ManifestShouldContainPropertyEditorUiExtension()
        {
            _creator.CreatePropertyEditors(new List<YamlPropertyEditor>
            {
                new YamlPropertyEditor { Alias = "Acme.Rating", Name = "Rating", UiAlias = "Acme.Rating.Ui" }
            });

            var manifestPath = Path.Combine(_wwwroot, "App_Plugins", "acme-rating", "umbraco-package.json");
            var json = File.ReadAllText(manifestPath);
            var doc = JsonDocument.Parse(json);

            var extensions = doc.RootElement.GetProperty("extensions");
            bool foundUi = false;
            foreach (var ext in extensions.EnumerateArray())
            {
                if (ext.GetProperty("type").GetString() == "propertyEditorUi"
                    && ext.GetProperty("alias").GetString() == "Acme.Rating.Ui")
                {
                    foundUi = true;
                    break;
                }
            }
            Assert.True(foundUi, "Should have a propertyEditorUi extension with the custom UI alias");
        }

        [Fact]
        public void CreatePropertyEditors_ShouldUseFolderNameWhenProvided()
        {
            _creator.CreatePropertyEditors(new List<YamlPropertyEditor>
            {
                new YamlPropertyEditor
                {
                    Alias = "My.Editor",
                    Name = "My Editor",
                    FolderName = "custom-folder"
                }
            });

            var manifestPath = Path.Combine(_wwwroot, "App_Plugins", "custom-folder", "umbraco-package.json");
            Assert.True(File.Exists(manifestPath));
        }

        [Fact]
        public void CreatePropertyEditors_ShouldDeriveFolderFromAliasWhenFolderNameNotSet()
        {
            _creator.CreatePropertyEditors(new List<YamlPropertyEditor>
            {
                new YamlPropertyEditor { Alias = "Acme.My.Editor" }
            });

            // dots replaced with dashes, lowercase
            var manifestPath = Path.Combine(_wwwroot, "App_Plugins", "acme-my-editor", "umbraco-package.json");
            Assert.True(File.Exists(manifestPath));
        }

        // ── Skip if exists (no update) ────────────────────────────────────────

        [Fact]
        public void CreatePropertyEditors_ShouldSkipIfManifestExistsWithoutUpdateFlag()
        {
            var pluginDir = Path.Combine(_wwwroot, "App_Plugins", "my-editor");
            Directory.CreateDirectory(pluginDir);
            var manifestPath = Path.Combine(pluginDir, "umbraco-package.json");
            File.WriteAllText(manifestPath, "{ \"original\": true }");

            _creator.CreatePropertyEditors(new List<YamlPropertyEditor>
            {
                new YamlPropertyEditor { Alias = "My.Editor", Name = "My Editor" }
            });

            var content = File.ReadAllText(manifestPath);
            Assert.Contains("original", content);
        }

        [Fact]
        public void CreatePropertyEditors_ShouldOverwriteIfManifestExistsWithUpdateFlag()
        {
            var pluginDir = Path.Combine(_wwwroot, "App_Plugins", "my-editor");
            Directory.CreateDirectory(pluginDir);
            var manifestPath = Path.Combine(pluginDir, "umbraco-package.json");
            File.WriteAllText(manifestPath, "{ \"original\": true }");

            _creator.CreatePropertyEditors(new List<YamlPropertyEditor>
            {
                new YamlPropertyEditor { Alias = "My.Editor", Name = "My Editor Updated", Update = true }
            });

            var content = File.ReadAllText(manifestPath);
            Assert.DoesNotContain("\"original\"", content);
            Assert.Contains("My Editor Updated", content);
        }

        // ── Remove ────────────────────────────────────────────────────────────

        [Fact]
        public void CreatePropertyEditors_RemoveShouldDeleteDirectory()
        {
            var pluginDir = Path.Combine(_wwwroot, "App_Plugins", "my-editor");
            Directory.CreateDirectory(pluginDir);
            File.WriteAllText(Path.Combine(pluginDir, "umbraco-package.json"), "{}");

            _creator.CreatePropertyEditors(new List<YamlPropertyEditor>
            {
                new YamlPropertyEditor { Alias = "My.Editor", Remove = true }
            });

            Assert.False(Directory.Exists(pluginDir));
        }

        [Fact]
        public void CreatePropertyEditors_RemoveShouldNotThrowWhenDirectoryMissing()
        {
            var ex = Record.Exception(() => _creator.CreatePropertyEditors(new List<YamlPropertyEditor>
            {
                new YamlPropertyEditor { Alias = "NonExistent.Editor", Remove = true }
            }));

            Assert.Null(ex);
        }

        [Fact]
        public void CreatePropertyEditors_RemoveShouldNotCreateManifest()
        {
            _creator.CreatePropertyEditors(new List<YamlPropertyEditor>
            {
                new YamlPropertyEditor { Alias = "My.Editor", Remove = true }
            });

            var pluginDir = Path.Combine(_wwwroot, "App_Plugins", "my-editor");
            Assert.False(Directory.Exists(pluginDir));
        }

        // ── Inline JS content ─────────────────────────────────────────────────

        [Fact]
        public void CreatePropertyEditors_ShouldWriteJsContentWhenProvided()
        {
            const string jsContent = "customElements.define('my-editor', class extends HTMLElement {});";

            _creator.CreatePropertyEditors(new List<YamlPropertyEditor>
            {
                new YamlPropertyEditor
                {
                    Alias = "My.Editor",
                    Name = "My Editor",
                    JsContent = jsContent
                }
            });

            var jsPath = Path.Combine(_wwwroot, "App_Plugins", "my-editor", "index.js");
            Assert.True(File.Exists(jsPath));
            Assert.Equal(jsContent, File.ReadAllText(jsPath));
        }

        [Fact]
        public void CreatePropertyEditors_ShouldWriteJsToCustomPathWhenJsPathProvided()
        {
            _creator.CreatePropertyEditors(new List<YamlPropertyEditor>
            {
                new YamlPropertyEditor
                {
                    Alias = "My.Editor",
                    Name = "My Editor",
                    JsPath = "/App_Plugins/my-editor/custom.js",
                    JsContent = "// custom"
                }
            });

            var jsPath = Path.Combine(_wwwroot, "App_Plugins", "my-editor", "custom.js");
            Assert.True(File.Exists(jsPath));
        }

        [Fact]
        public void CreatePropertyEditors_ShouldSkipJsWriteIfJsExistsWithoutUpdateFlag()
        {
            var pluginDir = Path.Combine(_wwwroot, "App_Plugins", "my-editor");
            Directory.CreateDirectory(pluginDir);
            var jsPath = Path.Combine(pluginDir, "index.js");
            File.WriteAllText(jsPath, "// original");

            // Already has the manifest too (so it won't overwrite)
            File.WriteAllText(Path.Combine(pluginDir, "umbraco-package.json"), "{\"name\":\"x\"}");

            _creator.CreatePropertyEditors(new List<YamlPropertyEditor>
            {
                new YamlPropertyEditor
                {
                    Alias = "My.Editor",
                    Name = "My Editor",
                    JsContent = "// new content",
                    Update = false
                }
            });

            // Manifest already exists + no update → creator skips entirely
            Assert.Equal("// original", File.ReadAllText(jsPath));
        }

        [Fact]
        public void CreatePropertyEditors_ShouldOverwriteJsWithUpdateFlag()
        {
            var pluginDir = Path.Combine(_wwwroot, "App_Plugins", "my-editor");
            Directory.CreateDirectory(pluginDir);
            var jsPath = Path.Combine(pluginDir, "index.js");
            File.WriteAllText(jsPath, "// original");
            File.WriteAllText(Path.Combine(pluginDir, "umbraco-package.json"), "{\"name\":\"x\"}");

            _creator.CreatePropertyEditors(new List<YamlPropertyEditor>
            {
                new YamlPropertyEditor
                {
                    Alias = "My.Editor",
                    Name = "My Editor",
                    JsContent = "// updated",
                    Update = true
                }
            });

            Assert.Equal("// updated", File.ReadAllText(jsPath));
        }

        // ── Missing alias ─────────────────────────────────────────────────────

        [Fact]
        public void CreatePropertyEditors_ShouldSkipEntryWithMissingAlias()
        {
            var ex = Record.Exception(() => _creator.CreatePropertyEditors(new List<YamlPropertyEditor>
            {
                new YamlPropertyEditor { Alias = null, Name = "No Alias" }
            }));

            Assert.Null(ex);
            Assert.False(Directory.Exists(Path.Combine(_wwwroot, "App_Plugins")));
        }

        [Fact]
        public void CreatePropertyEditors_ShouldSkipEntryWithWhitespaceAlias()
        {
            var ex = Record.Exception(() => _creator.CreatePropertyEditors(new List<YamlPropertyEditor>
            {
                new YamlPropertyEditor { Alias = "   ", Name = "Whitespace Alias" }
            }));

            Assert.Null(ex);
        }

        // ── Default icon / group ──────────────────────────────────────────────

        [Fact]
        public void CreatePropertyEditors_ShouldUseDefaultIconAndGroup()
        {
            _creator.CreatePropertyEditors(new List<YamlPropertyEditor>
            {
                new YamlPropertyEditor { Alias = "My.Editor" }
            });

            var manifestPath = Path.Combine(_wwwroot, "App_Plugins", "my-editor", "umbraco-package.json");
            var json = File.ReadAllText(manifestPath);

            Assert.Contains("icon-code", json);
            Assert.Contains("common", json);
        }

        [Fact]
        public void CreatePropertyEditors_ShouldUseCustomIconAndGroup()
        {
            _creator.CreatePropertyEditors(new List<YamlPropertyEditor>
            {
                new YamlPropertyEditor { Alias = "My.Editor", Icon = "icon-star", Group = "media" }
            });

            var manifestPath = Path.Combine(_wwwroot, "App_Plugins", "my-editor", "umbraco-package.json");
            var json = File.ReadAllText(manifestPath);

            Assert.Contains("icon-star", json);
            Assert.Contains("media", json);
        }

        // ── Derived UI alias ──────────────────────────────────────────────────

        [Fact]
        public void CreatePropertyEditors_ShouldDeriveUiAliasFromAliasWhenNotSet()
        {
            _creator.CreatePropertyEditors(new List<YamlPropertyEditor>
            {
                new YamlPropertyEditor { Alias = "My.Editor" }
            });

            var manifestPath = Path.Combine(_wwwroot, "App_Plugins", "my-editor", "umbraco-package.json");
            var json = File.ReadAllText(manifestPath);
            var doc = JsonDocument.Parse(json);

            var extensions = doc.RootElement.GetProperty("extensions");
            string? foundUiAlias = null;
            foreach (var ext in extensions.EnumerateArray())
            {
                if (ext.GetProperty("type").GetString() == "propertyEditorUi")
                {
                    foundUiAlias = ext.GetProperty("alias").GetString();
                    break;
                }
            }

            Assert.Equal("My.Editor.Ui", foundUiAlias);
        }
    }
}
