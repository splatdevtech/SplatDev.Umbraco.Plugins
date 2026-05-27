using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Services;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Tests
{
    public class StaticAssetCreatorTests : IDisposable
    {
        private readonly string _wwwroot;
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly StaticAssetCreator _creator;

        public StaticAssetCreatorTests()
        {
            _wwwroot = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_wwwroot);

            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockEnv.Setup(x => x.WebRootPath).Returns(_wwwroot);

            _creator = new StaticAssetCreator(_mockEnv.Object);
        }

        public void Dispose()
        {
            if (Directory.Exists(_wwwroot))
                Directory.Delete(_wwwroot, recursive: true);
        }

        // ── Scripts ────────────────────────────────────────────────────────────

        [Fact]
        public void CreateScripts_ShouldWriteFileToWwwroot()
        {
            var scripts = new List<YamlScript>
            {
                new YamlScript { Alias = "site", Path = "js/site.js", Content = "console.log('hi');" }
            };

            _creator.CreateScripts(scripts);

            var fullPath = Path.Combine(_wwwroot, "js", "site.js");
            Assert.True(File.Exists(fullPath));
            Assert.Equal("console.log('hi');", File.ReadAllText(fullPath));
        }

        [Fact]
        public void CreateScripts_ShouldCreateSubdirectories()
        {
            var scripts = new List<YamlScript>
            {
                new YamlScript { Alias = "deep", Path = "assets/js/app/main.js", Content = "// main" }
            };

            _creator.CreateScripts(scripts);

            Assert.True(File.Exists(Path.Combine(_wwwroot, "assets", "js", "app", "main.js")));
        }

        [Fact]
        public void CreateScripts_ShouldSkipExistingFile()
        {
            var jsDir = Path.Combine(_wwwroot, "js");
            Directory.CreateDirectory(jsDir);
            var filePath = Path.Combine(jsDir, "site.js");
            File.WriteAllText(filePath, "original");

            var scripts = new List<YamlScript>
            {
                new YamlScript { Alias = "site", Path = "js/site.js", Content = "overwrite attempt" }
            };

            _creator.CreateScripts(scripts);

            Assert.Equal("original", File.ReadAllText(filePath));
        }

        [Fact]
        public void CreateScripts_ShouldOverwriteExistingFileWhenUpdateFlagSet()
        {
            var jsDir = Path.Combine(_wwwroot, "js");
            Directory.CreateDirectory(jsDir);
            var filePath = Path.Combine(jsDir, "site.js");
            File.WriteAllText(filePath, "old content");

            var scripts = new List<YamlScript>
            {
                new YamlScript { Alias = "site", Path = "js/site.js", Content = "new content", Update = true }
            };

            _creator.CreateScripts(scripts);

            Assert.Equal("new content", File.ReadAllText(filePath));
        }

        [Fact]
        public void CreateScripts_ShouldDeleteFileWhenRemoveFlagSet()
        {
            var jsDir = Path.Combine(_wwwroot, "js");
            Directory.CreateDirectory(jsDir);
            var filePath = Path.Combine(jsDir, "legacy.js");
            File.WriteAllText(filePath, "// legacy");

            var scripts = new List<YamlScript>
            {
                new YamlScript { Alias = "legacy", Path = "js/legacy.js", Remove = true }
            };

            _creator.CreateScripts(scripts);

            Assert.False(File.Exists(filePath));
        }

        [Fact]
        public void CreateScripts_ShouldNotThrowWhenRemoveTargetMissing()
        {
            var scripts = new List<YamlScript>
            {
                new YamlScript { Alias = "gone", Path = "js/gone.js", Remove = true }
            };

            // Should log warning and not throw
            var ex = Record.Exception(() => _creator.CreateScripts(scripts));
            Assert.Null(ex);
        }

        [Fact]
        public void CreateScripts_ShouldSkipDuplicateAliases()
        {
            var scripts = new List<YamlScript>
            {
                new YamlScript { Alias = "dup", Path = "js/dup.js", Content = "first" },
                new YamlScript { Alias = "dup", Path = "js/dup2.js", Content = "second" }
            };

            _creator.CreateScripts(scripts);

            Assert.True(File.Exists(Path.Combine(_wwwroot, "js", "dup.js")));
            Assert.False(File.Exists(Path.Combine(_wwwroot, "js", "dup2.js")));
        }

        [Fact]
        public void CreateScripts_ShouldSkipItemWithMissingAlias()
        {
            var scripts = new List<YamlScript>
            {
                new YamlScript { Alias = null, Path = "js/noalias.js", Content = "x" }
            };

            var ex = Record.Exception(() => _creator.CreateScripts(scripts));
            Assert.Null(ex);
            Assert.False(File.Exists(Path.Combine(_wwwroot, "js", "noalias.js")));
        }

        [Fact]
        public void CreateScripts_ShouldSkipItemWithEmptyPath()
        {
            var scripts = new List<YamlScript>
            {
                new YamlScript { Alias = "nopath", Path = null, Content = "x" }
            };

            var ex = Record.Exception(() => _creator.CreateScripts(scripts));
            Assert.Null(ex);
        }

        [Fact]
        public void CreateScripts_ShouldThrowOnNullList()
        {
            Assert.Throws<ArgumentNullException>(() => _creator.CreateScripts(null!));
        }

        // ── Stylesheets ────────────────────────────────────────────────────────

        [Fact]
        public void CreateStylesheets_ShouldWriteFileToWwwroot()
        {
            var stylesheets = new List<YamlStylesheet>
            {
                new YamlStylesheet { Alias = "site", Path = "css/site.css", Content = "body { margin: 0; }" }
            };

            _creator.CreateStylesheets(stylesheets);

            var fullPath = Path.Combine(_wwwroot, "css", "site.css");
            Assert.True(File.Exists(fullPath));
            Assert.Equal("body { margin: 0; }", File.ReadAllText(fullPath));
        }

        [Fact]
        public void CreateStylesheets_ShouldSkipExistingFile()
        {
            var cssDir = Path.Combine(_wwwroot, "css");
            Directory.CreateDirectory(cssDir);
            var filePath = Path.Combine(cssDir, "site.css");
            File.WriteAllText(filePath, "original-css");

            var stylesheets = new List<YamlStylesheet>
            {
                new YamlStylesheet { Alias = "site", Path = "css/site.css", Content = "new-css" }
            };

            _creator.CreateStylesheets(stylesheets);

            Assert.Equal("original-css", File.ReadAllText(filePath));
        }

        [Fact]
        public void CreateStylesheets_ShouldOverwriteExistingFileWhenUpdateFlagSet()
        {
            var cssDir = Path.Combine(_wwwroot, "css");
            Directory.CreateDirectory(cssDir);
            var filePath = Path.Combine(cssDir, "site.css");
            File.WriteAllText(filePath, "old-css");

            var stylesheets = new List<YamlStylesheet>
            {
                new YamlStylesheet { Alias = "site", Path = "css/site.css", Content = "new-css", Update = true }
            };

            _creator.CreateStylesheets(stylesheets);

            Assert.Equal("new-css", File.ReadAllText(filePath));
        }

        [Fact]
        public void CreateStylesheets_ShouldDeleteFileWhenRemoveFlagSet()
        {
            var cssDir = Path.Combine(_wwwroot, "css");
            Directory.CreateDirectory(cssDir);
            var filePath = Path.Combine(cssDir, "old.css");
            File.WriteAllText(filePath, "/* old */");

            var stylesheets = new List<YamlStylesheet>
            {
                new YamlStylesheet { Alias = "old", Path = "css/old.css", Remove = true }
            };

            _creator.CreateStylesheets(stylesheets);

            Assert.False(File.Exists(filePath));
        }

        [Fact]
        public void CreateStylesheets_ShouldNotThrowWhenRemoveTargetMissing()
        {
            var stylesheets = new List<YamlStylesheet>
            {
                new YamlStylesheet { Alias = "gone", Path = "css/gone.css", Remove = true }
            };

            var ex = Record.Exception(() => _creator.CreateStylesheets(stylesheets));
            Assert.Null(ex);
        }

        [Fact]
        public void CreateStylesheets_ShouldSkipDuplicateAliases()
        {
            var stylesheets = new List<YamlStylesheet>
            {
                new YamlStylesheet { Alias = "dup", Path = "css/dup.css", Content = "first" },
                new YamlStylesheet { Alias = "dup", Path = "css/dup2.css", Content = "second" }
            };

            _creator.CreateStylesheets(stylesheets);

            Assert.True(File.Exists(Path.Combine(_wwwroot, "css", "dup.css")));
            Assert.False(File.Exists(Path.Combine(_wwwroot, "css", "dup2.css")));
        }

        [Fact]
        public void CreateStylesheets_ShouldThrowOnNullList()
        {
            Assert.Throws<ArgumentNullException>(() => _creator.CreateStylesheets(null!));
        }

        // ── Leading slash normalisation ────────────────────────────────────────

        [Fact]
        public void CreateScripts_ShouldNormaliseLeadingSlashInPath()
        {
            var scripts = new List<YamlScript>
            {
                new YamlScript { Alias = "slash", Path = "/js/slashed.js", Content = "ok" }
            };

            _creator.CreateScripts(scripts);

            Assert.True(File.Exists(Path.Combine(_wwwroot, "js", "slashed.js")));
        }
    }
}
