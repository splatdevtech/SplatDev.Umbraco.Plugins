using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Services;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Tests
{
    public class ContentCreatorTests
    {
        private (Mock<IContentService>, Mock<IContentTypeService>, ContentCreator) Build()
        {
            var mockContentService = new Mock<IContentService>();
            var mockContentTypeService = new Mock<IContentTypeService>();
            var mockMediaService = new Mock<IMediaService>();
            var creator = new ContentCreator(mockContentService.Object, mockContentTypeService.Object, mockMediaService.Object);
            return (mockContentService, mockContentTypeService, creator);
        }

        // ── CREATE ────────────────────────────────────────────────────────────

        [Fact]
        public void CreateContent_ShouldCreateFromYaml()
        {
            var (mockContentService, mockContentTypeService, creator) = Build();

            var contentType = new Mock<IContentType>();
            contentType.Setup(x => x.Id).Returns(1);
            contentType.Setup(x => x.Alias).Returns("page");
            mockContentTypeService.Setup(x => x.Get(It.IsAny<string>())).Returns(contentType.Object);

            var mockContent = new Mock<IContent>();
            mockContent.Setup(x => x.Properties).Returns(new PropertyCollection());
            mockContentService
                .Setup(x => x.Create(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(mockContent.Object);

            creator.CreateContent(new List<YamlContent>
            {
                new YamlContent { Alias = "home", Name = "Home", Type = "page", Published = true, Values = new() { { "title", "Welcome" } } }
            });

            mockContentService.Verify(x =>
                x.Save(It.IsAny<IContent>(), It.IsAny<int?>(), It.IsAny<ContentScheduleCollection>()), Times.Once);
        }

        // ── REMOVE ────────────────────────────────────────────────────────────

        [Fact]
        public void CreateContent_ShouldRemoveExistingContent()
        {
            var (mockContentService, _, creator) = Build();

            var mockNode = new Mock<IContent>();
            mockNode.Setup(x => x.Name).Returns("Home");

            mockContentService.Setup(x => x.GetRootContent()).Returns(new[] { mockNode.Object });

            creator.CreateContent(new List<YamlContent>
            {
                new YamlContent { Alias = "home", Name = "Home", Remove = true }
            });

            mockContentService.Verify(x => x.Delete(mockNode.Object, It.IsAny<int>()), Times.Once);
            mockContentService.Verify(x =>
                x.Save(It.IsAny<IContent>(), It.IsAny<int?>(), It.IsAny<ContentScheduleCollection>()),
                Times.Never);
        }

        [Fact]
        public void CreateContent_ShouldNotThrowWhenRemoveTargetMissing()
        {
            var (mockContentService, _, creator) = Build();
            mockContentService.Setup(x => x.GetRootContent()).Returns(Array.Empty<IContent>());

            var ex = Record.Exception(() => creator.CreateContent(new List<YamlContent>
            {
                new YamlContent { Alias = "gone", Name = "Gone", Remove = true }
            }));

            Assert.Null(ex);
            mockContentService.Verify(x => x.Delete(It.IsAny<IContent>(), It.IsAny<int>()), Times.Never);
        }

        // ── UPDATE ────────────────────────────────────────────────────────────

        [Fact]
        public void CreateContent_ShouldUpdateExistingContent()
        {
            var (mockContentService, _, creator) = Build();

            var mockNode = new Mock<IContent>();
            mockNode.Setup(x => x.Name).Returns("Home");
            mockNode.Setup(x => x.Id).Returns(1);
            mockNode.Setup(x => x.TemplateId).Returns(1); // skip template restoration branch
            mockNode.Setup(x => x.Properties).Returns(new PropertyCollection());

            mockContentService.Setup(x => x.GetRootContent()).Returns(new[] { mockNode.Object });

            creator.CreateContent(new List<YamlContent>
            {
                new YamlContent { Alias = "home", Name = "Home", Update = true, Values = new() }
            });

            mockContentService.Verify(x =>
                x.Save(mockNode.Object, It.IsAny<int?>(), It.IsAny<ContentScheduleCollection>()),
                Times.Once);
        }

        [Fact]
        public void CreateContent_ShouldSkipWhenUpdateTargetMissing()
        {
            // ContentCreator.update:true means "update if found, skip if not found" — it does NOT create.
            var (mockContentService, _, creator) = Build();

            mockContentService.Setup(x => x.GetRootContent()).Returns(Array.Empty<IContent>());

            creator.CreateContent(new List<YamlContent>
            {
                new YamlContent { Alias = "home", Name = "Home", Type = "page", Update = true, Values = new() }
            });

            // Not found + update:true → skip (no Save, no Create)
            mockContentService.Verify(x =>
                x.Save(It.IsAny<IContent>(), It.IsAny<int?>(), It.IsAny<ContentScheduleCollection>()),
                Times.Never);
            mockContentService.Verify(x =>
                x.Create(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Never);
        }
    }
}
