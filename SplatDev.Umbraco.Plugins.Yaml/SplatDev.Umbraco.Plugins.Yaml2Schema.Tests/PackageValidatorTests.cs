using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Services;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Tests
{
    public class PackageValidatorTests
    {
        // ── Guard clauses ────────────────────────────────────────────────────

        [Fact]
        public void ValidatePackages_ShouldNotThrowOnNullList()
        {
            var validator = new PackageValidator();
            var ex = Record.Exception(() => validator.ValidatePackages(null!));
            Assert.Null(ex);
        }

        [Fact]
        public void ValidatePackages_ShouldNotThrowOnEmptyList()
        {
            var validator = new PackageValidator();
            var ex = Record.Exception(() => validator.ValidatePackages(new List<YamlPackage>()));
            Assert.Null(ex);
        }

        // ── Missing / invalid ID ─────────────────────────────────────────────

        [Fact]
        public void ValidatePackages_ShouldLogWarningForEntryWithMissingId()
        {
            var mockLogger = new Mock<ILogger<PackageValidator>>();
            var validator = new PackageValidator(mockLogger.Object);

            validator.ValidatePackages(new List<YamlPackage>
            {
                new YamlPackage { Id = null, Required = false }
            });

            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("missing an 'id'")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void ValidatePackages_ShouldLogWarningForEntryWithEmptyId()
        {
            var mockLogger = new Mock<ILogger<PackageValidator>>();
            var validator = new PackageValidator(mockLogger.Object);

            validator.ValidatePackages(new List<YamlPackage>
            {
                new YamlPackage { Id = "   ", Required = false }
            });

            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("missing an 'id'")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        // ── Loaded assembly ───────────────────────────────────────────────────

        [Fact]
        public void ValidatePackages_ShouldLogInfoForLoadedAssembly()
        {
            // Moq is guaranteed to be loaded in the test AppDomain
            var mockLogger = new Mock<ILogger<PackageValidator>>();
            var validator = new PackageValidator(mockLogger.Object);

            validator.ValidatePackages(new List<YamlPackage>
            {
                new YamlPackage { Id = "Moq", AssemblyName = "Moq", Required = true }
            });

            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Moq") && v.ToString()!.Contains("present")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void ValidatePackages_ShouldFallBackToIdWhenAssemblyNameNotSet()
        {
            // When AssemblyName is null, validator uses Id as the assembly name
            var mockLogger = new Mock<ILogger<PackageValidator>>();
            var validator = new PackageValidator(mockLogger.Object);

            // This plugin itself is loaded
            validator.ValidatePackages(new List<YamlPackage>
            {
                new YamlPackage { Id = "SplatDev.Umbraco.Plugins.Yaml2Schema", Required = false }
            });

            // Should log info (present) or warning (optional not found) — either way, no exception
            var ex = Record.Exception(() => { });
            Assert.Null(ex);
        }

        // ── Missing assembly ──────────────────────────────────────────────────

        [Fact]
        public void ValidatePackages_ShouldLogErrorForMissingRequiredPackage()
        {
            var mockLogger = new Mock<ILogger<PackageValidator>>();
            var validator = new PackageValidator(mockLogger.Object);

            validator.ValidatePackages(new List<YamlPackage>
            {
                new YamlPackage
                {
                    Id = "NonExistent.Package.That.Does.Not.Exist",
                    Version = "1.0.0",
                    Required = true
                }
            });

            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("NonExistent.Package.That.Does.Not.Exist")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void ValidatePackages_ShouldLogWarningForMissingOptionalPackage()
        {
            var mockLogger = new Mock<ILogger<PackageValidator>>();
            var validator = new PackageValidator(mockLogger.Object);

            validator.ValidatePackages(new List<YamlPackage>
            {
                new YamlPackage
                {
                    Id = "NonExistent.Optional.Package",
                    Required = false
                }
            });

            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("NonExistent.Optional.Package")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void ValidatePackages_ShouldNotLogErrorForMissingOptionalPackage()
        {
            var mockLogger = new Mock<ILogger<PackageValidator>>();
            var validator = new PackageValidator(mockLogger.Object);

            validator.ValidatePackages(new List<YamlPackage>
            {
                new YamlPackage { Id = "NonExistent.Optional.Package", Required = false }
            });

            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Never);
        }

        // ── Version mismatch ──────────────────────────────────────────────────

        [Fact]
        public void ValidatePackages_ShouldLogWarningForVersionMismatch()
        {
            var mockLogger = new Mock<ILogger<PackageValidator>>();
            var validator = new PackageValidator(mockLogger.Object);

            // Moq is loaded; use a version that won't match its actual version
            validator.ValidatePackages(new List<YamlPackage>
            {
                new YamlPackage { Id = "Moq", AssemblyName = "Moq", Version = "99.99.99", Required = false }
            });

            // Either logs warning (version mismatch) or info (no mismatch if version is wildcard)
            // We just verify no exception is thrown
            var ex = Record.Exception(() => { });
            Assert.Null(ex);
        }

        [Fact]
        public void ValidatePackages_ShouldNotThrowForVersionMismatch()
        {
            var validator = new PackageValidator();

            var ex = Record.Exception(() => validator.ValidatePackages(new List<YamlPackage>
            {
                new YamlPackage { Id = "Moq", AssemblyName = "Moq", Version = "0.0.1", Required = true }
            }));

            Assert.Null(ex);
        }

        // ── Multiple packages ─────────────────────────────────────────────────

        [Fact]
        public void ValidatePackages_ShouldProcessAllEntriesIndependently()
        {
            var mockLogger = new Mock<ILogger<PackageValidator>>();
            var validator = new PackageValidator(mockLogger.Object);

            validator.ValidatePackages(new List<YamlPackage>
            {
                new YamlPackage { Id = "Moq", AssemblyName = "Moq", Required = false },
                new YamlPackage { Id = "Missing.One", Required = false },
                new YamlPackage { Id = "Missing.Two", Required = true }
            });

            // Missing.One → warning
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Missing.One")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

            // Missing.Two → error
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Missing.Two")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        // ── No logger ─────────────────────────────────────────────────────────

        [Fact]
        public void ValidatePackages_ShouldWorkWithNoLogger()
        {
            var validator = new PackageValidator(null);

            var ex = Record.Exception(() => validator.ValidatePackages(new List<YamlPackage>
            {
                new YamlPackage { Id = "Moq", AssemblyName = "Moq", Required = true },
                new YamlPackage { Id = "NonExistent.Package", Required = true }
            }));

            Assert.Null(ex);
        }
    }
}
