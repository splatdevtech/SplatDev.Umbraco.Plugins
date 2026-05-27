using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Microsoft.Extensions.Logging;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Services;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Tests
{
    public class MediaCreatorTests
    {
        // MediaFileManager is sealed and cannot be mocked via Moq.
        // RuntimeHelpers.GetUninitializedObject creates an instance without invoking the constructor.
        // This is safe because tests here never set Url on YamlMedia, so TryAttachFileFromUrl
        // (which is the only method that uses _mediaFileManager) is never called.
        private static readonly MediaFileManager _uninitializedFileManager =
            (MediaFileManager)RuntimeHelpers.GetUninitializedObject(typeof(MediaFileManager));

        private static MediaCreator BuildCreator(
            Mock<IMediaService>? mockMediaService = null,
            Mock<IMediaTypeService>? mockMediaTypeService = null)
        {
            mockMediaService ??= new Mock<IMediaService>();
            mockMediaTypeService ??= new Mock<IMediaTypeService>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();

            return new MediaCreator(
                mockMediaService.Object,
                mockMediaTypeService.Object,
                _uninitializedFileManager,
                mockHttpClientFactory.Object);
        }

        private static Mock<IMedia> MakeMedia(string name, int id = 1)
        {
            var mock = new Mock<IMedia>();
            mock.Setup(x => x.Name).Returns(name);
            mock.Setup(x => x.Id).Returns(id);
            mock.Setup(x => x.Properties).Returns(new PropertyCollection());

            var mockContentType = new Mock<ISimpleContentType>();
            mockContentType.Setup(x => x.Alias).Returns("Image");
            mock.Setup(x => x.ContentType).Returns(mockContentType.Object);

            return mock;
        }

        private static Mock<IMedia> MakeFolder(string name, int id, string alias = "Folder")
        {
            var mock = new Mock<IMedia>();
            mock.Setup(x => x.Name).Returns(name);
            mock.Setup(x => x.Id).Returns(id);
            mock.Setup(x => x.Properties).Returns(new PropertyCollection());

            var mockContentType = new Mock<ISimpleContentType>();
            mockContentType.Setup(x => x.Alias).Returns(alias);
            mock.Setup(x => x.ContentType).Returns(mockContentType.Object);

            return mock;
        }

        // ── Guard clauses ────────────────────────────────────────────────────

        [Fact]
        public void CreateMedia_ShouldThrowOnNullList()
        {
            var creator = BuildCreator();
            Assert.Throws<ArgumentNullException>(() => creator.CreateMedia(null!));
        }

        [Fact]
        public void CreateMedia_ShouldSkipEntryWithEmptyName()
        {
            var mockMediaService = new Mock<IMediaService>();
            mockMediaService.Setup(x => x.GetRootMedia()).Returns(Array.Empty<IMedia>());
            var creator = BuildCreator(mockMediaService);

            var ex = Record.Exception(() => creator.CreateMedia(new List<YamlMedia>
            {
                new YamlMedia { Name = null, MediaType = "Image" }
            }));

            Assert.Null(ex);
            mockMediaService.Verify(x => x.CreateMedia(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void CreateMedia_ShouldSkipEntryWithWhitespaceName()
        {
            var mockMediaService = new Mock<IMediaService>();
            mockMediaService.Setup(x => x.GetRootMedia()).Returns(Array.Empty<IMedia>());
            var creator = BuildCreator(mockMediaService);

            var ex = Record.Exception(() => creator.CreateMedia(new List<YamlMedia>
            {
                new YamlMedia { Name = "   ", MediaType = "Image" }
            }));

            Assert.Null(ex);
            mockMediaService.Verify(x => x.CreateMedia(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        // ── CREATE ────────────────────────────────────────────────────────────

        [Fact]
        public void CreateMedia_ShouldCreateAndSaveMedia()
        {
            var mockMediaService = new Mock<IMediaService>();
            mockMediaService.Setup(x => x.GetRootMedia()).Returns(Array.Empty<IMedia>());

            var newMedia = MakeMedia("Banner");
            mockMediaService
                .Setup(x => x.CreateMedia("Banner", -1, "Image", Constants.Security.SuperUserId))
                .Returns(newMedia.Object);

            var creator = BuildCreator(mockMediaService);

            creator.CreateMedia(new List<YamlMedia>
            {
                new YamlMedia { Name = "Banner", MediaType = "Image" }
            });

            mockMediaService.Verify(
                x => x.Save(newMedia.Object, Constants.Security.SuperUserId),
                Times.Once);
        }

        [Fact]
        public void CreateMedia_ShouldSkipIfAlreadyExists()
        {
            var existing = MakeMedia("Banner", 10);
            var mockMediaService = new Mock<IMediaService>();
            mockMediaService.Setup(x => x.GetRootMedia()).Returns(new[] { existing.Object });

            var creator = BuildCreator(mockMediaService);

            creator.CreateMedia(new List<YamlMedia>
            {
                new YamlMedia { Name = "Banner", MediaType = "Image" }
            });

            mockMediaService.Verify(
                x => x.CreateMedia(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Never);

            mockMediaService.Verify(
                x => x.Save(It.IsAny<IMedia>(), It.IsAny<int>()),
                Times.Never);
        }

        // ── REMOVE ────────────────────────────────────────────────────────────

        [Fact]
        public void CreateMedia_ShouldRemoveExistingMedia()
        {
            var existing = MakeMedia("OldBanner", 5);
            var mockMediaService = new Mock<IMediaService>();
            mockMediaService.Setup(x => x.GetRootMedia()).Returns(new[] { existing.Object });

            var creator = BuildCreator(mockMediaService);

            creator.CreateMedia(new List<YamlMedia>
            {
                new YamlMedia { Name = "OldBanner", Remove = true }
            });

            mockMediaService.Verify(
                x => x.Delete(existing.Object, Constants.Security.SuperUserId),
                Times.Once);

            mockMediaService.Verify(
                x => x.Save(It.IsAny<IMedia>(), It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public void CreateMedia_ShouldNotThrowWhenRemoveTargetMissing()
        {
            var mockMediaService = new Mock<IMediaService>();
            mockMediaService.Setup(x => x.GetRootMedia()).Returns(Array.Empty<IMedia>());

            var creator = BuildCreator(mockMediaService);

            var ex = Record.Exception(() => creator.CreateMedia(new List<YamlMedia>
            {
                new YamlMedia { Name = "NonExistentMedia", Remove = true }
            }));

            Assert.Null(ex);
            mockMediaService.Verify(x => x.Delete(It.IsAny<IMedia>(), It.IsAny<int>()), Times.Never);
        }

        // ── UPDATE ────────────────────────────────────────────────────────────

        [Fact]
        public void CreateMedia_ShouldUpdateExistingMedia()
        {
            var existing = MakeMedia("Banner", 10);
            var mockMediaService = new Mock<IMediaService>();
            mockMediaService.Setup(x => x.GetRootMedia()).Returns(new[] { existing.Object });

            var creator = BuildCreator(mockMediaService);

            creator.CreateMedia(new List<YamlMedia>
            {
                new YamlMedia { Name = "Banner", Update = true, Properties = new Dictionary<string, object>() }
            });

            mockMediaService.Verify(
                x => x.Save(existing.Object, Constants.Security.SuperUserId),
                Times.Once);
        }

        [Fact]
        public void CreateMedia_ShouldSkipWhenUpdateTargetMissing()
        {
            var mockMediaService = new Mock<IMediaService>();
            mockMediaService.Setup(x => x.GetRootMedia()).Returns(Array.Empty<IMedia>());

            var creator = BuildCreator(mockMediaService);

            creator.CreateMedia(new List<YamlMedia>
            {
                new YamlMedia { Name = "Missing", Update = true }
            });

            mockMediaService.Verify(x => x.Save(It.IsAny<IMedia>(), It.IsAny<int>()), Times.Never);
            mockMediaService.Verify(x => x.CreateMedia(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        // ── Folder creation (EnsureFolder) ────────────────────────────────────

        [Fact]
        public void CreateMedia_ShouldCreateFolderWhenFolderPathSpecified()
        {
            var mockMediaService = new Mock<IMediaService>();

            // Root media: no items (folder doesn't exist yet)
            mockMediaService.Setup(x => x.GetRootMedia()).Returns(Array.Empty<IMedia>());

            // Folder creation
            var folder = MakeFolder("Images", 100);
            mockMediaService
                .Setup(x => x.CreateMedia("Images", -1, "Folder", Constants.Security.SuperUserId))
                .Returns(folder.Object);

            // After folder is created, searching its children finds nothing (media doesn't exist)
            long total;
            mockMediaService
                .Setup(x => x.GetPagedChildren(100, 0, int.MaxValue, out total))
                .Returns(Array.Empty<IMedia>());

            // The actual media item
            var newMedia = MakeMedia("Logo", 200);
            mockMediaService
                .Setup(x => x.CreateMedia("Logo", 100, "Image", Constants.Security.SuperUserId))
                .Returns(newMedia.Object);

            var creator = BuildCreator(mockMediaService);

            creator.CreateMedia(new List<YamlMedia>
            {
                new YamlMedia { Name = "Logo", MediaType = "Image", Folder = "Images" }
            });

            // Folder should be saved
            mockMediaService.Verify(
                x => x.Save(folder.Object, Constants.Security.SuperUserId),
                Times.Once);

            // Media should be created under the folder
            mockMediaService.Verify(
                x => x.CreateMedia("Logo", 100, "Image", Constants.Security.SuperUserId),
                Times.Once);

            mockMediaService.Verify(
                x => x.Save(newMedia.Object, Constants.Security.SuperUserId),
                Times.Once);
        }

        [Fact]
        public void CreateMedia_ShouldReuseExistingFolderWhenPresent()
        {
            var mockMediaService = new Mock<IMediaService>();
            var existingFolder = MakeFolder("Images", 50);

            // Root returns existing folder
            mockMediaService.Setup(x => x.GetRootMedia()).Returns(new[] { existingFolder.Object });

            long total;
            // No existing media in folder
            mockMediaService
                .Setup(x => x.GetPagedChildren(50, 0, int.MaxValue, out total))
                .Returns(Array.Empty<IMedia>());

            var newMedia = MakeMedia("Logo", 201);
            mockMediaService
                .Setup(x => x.CreateMedia("Logo", 50, "Image", Constants.Security.SuperUserId))
                .Returns(newMedia.Object);

            var creator = BuildCreator(mockMediaService);

            creator.CreateMedia(new List<YamlMedia>
            {
                new YamlMedia { Name = "Logo", MediaType = "Image", Folder = "Images" }
            });

            // Folder already exists, should NOT be created again
            mockMediaService.Verify(
                x => x.CreateMedia("Images", It.IsAny<int>(), "Folder", It.IsAny<int>()),
                Times.Never);

            // Media created under existing folder
            mockMediaService.Verify(
                x => x.CreateMedia("Logo", 50, "Image", Constants.Security.SuperUserId),
                Times.Once);
        }

        [Fact]
        public void CreateMedia_ShouldHandleNestedFolderPath()
        {
            var mockMediaService = new Mock<IMediaService>();
            var parentFolder = MakeFolder("Brands", 10);
            var childFolder = MakeFolder("Logos", 20);

            mockMediaService.Setup(x => x.GetRootMedia()).Returns(Array.Empty<IMedia>());
            mockMediaService
                .Setup(x => x.CreateMedia("Brands", -1, "Folder", Constants.Security.SuperUserId))
                .Returns(parentFolder.Object);

            long total1, total2;
            mockMediaService
                .Setup(x => x.GetPagedChildren(10, 0, int.MaxValue, out total1))
                .Returns(Array.Empty<IMedia>());

            mockMediaService
                .Setup(x => x.CreateMedia("Logos", 10, "Folder", Constants.Security.SuperUserId))
                .Returns(childFolder.Object);

            mockMediaService
                .Setup(x => x.GetPagedChildren(20, 0, int.MaxValue, out total2))
                .Returns(Array.Empty<IMedia>());

            var newMedia = MakeMedia("Logo.png", 30);
            mockMediaService
                .Setup(x => x.CreateMedia("Logo.png", 20, "Image", Constants.Security.SuperUserId))
                .Returns(newMedia.Object);

            var creator = BuildCreator(mockMediaService);

            creator.CreateMedia(new List<YamlMedia>
            {
                new YamlMedia { Name = "Logo.png", MediaType = "Image", Folder = "Brands/Logos" }
            });

            // Both folder levels created
            mockMediaService.Verify(
                x => x.CreateMedia("Brands", -1, "Folder", Constants.Security.SuperUserId), Times.Once);
            mockMediaService.Verify(
                x => x.CreateMedia("Logos", 10, "Folder", Constants.Security.SuperUserId), Times.Once);

            // Media created in deepest folder
            mockMediaService.Verify(
                x => x.CreateMedia("Logo.png", 20, "Image", Constants.Security.SuperUserId), Times.Once);
        }

        // ── Nested children ───────────────────────────────────────────────────

        [Fact]
        public void CreateMedia_ShouldProcessChildrenRecursively()
        {
            var mockMediaService = new Mock<IMediaService>();

            var rootMedia = MakeMedia("Folder", 5);
            var childMedia = MakeMedia("Photo", 6);

            // Root lookup
            mockMediaService.Setup(x => x.GetRootMedia()).Returns(Array.Empty<IMedia>());
            mockMediaService
                .Setup(x => x.CreateMedia("Folder", -1, "Folder", Constants.Security.SuperUserId))
                .Returns(rootMedia.Object);

            long total;
            // Child lookup under parent
            mockMediaService
                .Setup(x => x.GetPagedChildren(5, 0, int.MaxValue, out total))
                .Returns(Array.Empty<IMedia>());
            mockMediaService
                .Setup(x => x.CreateMedia("Photo", 5, "Image", Constants.Security.SuperUserId))
                .Returns(childMedia.Object);

            var creator = BuildCreator(mockMediaService);

            creator.CreateMedia(new List<YamlMedia>
            {
                new YamlMedia
                {
                    Name = "Folder",
                    MediaType = "Folder",
                    Children = new List<YamlMedia>
                    {
                        new YamlMedia { Name = "Photo", MediaType = "Image" }
                    }
                }
            });

            mockMediaService.Verify(
                x => x.Save(rootMedia.Object, Constants.Security.SuperUserId), Times.Once);
            mockMediaService.Verify(
                x => x.Save(childMedia.Object, Constants.Security.SuperUserId), Times.Once);
        }
    }
}
