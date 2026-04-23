using Moq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;

namespace UmbracoCms.Tools.Tests;

public static class UmbracoContextMockFactory
{
    public static (Mock<IUmbracoContextFactory> Factory, Mock<IUmbracoContext> Context) Create(
        Action<Mock<IPublishedContentCache>>? configureCache = null)
    {
        var cacheMock = new Mock<IPublishedContentCache>();
        configureCache?.Invoke(cacheMock);

        var contextMock = new Mock<IUmbracoContext>();
        contextMock.Setup(c => c.Content).Returns(cacheMock.Object);

        var referenceMock = new Mock<IUmbracoContextReference>();
        referenceMock.Setup(r => r.UmbracoContext).Returns(contextMock.Object);

        var factoryMock = new Mock<IUmbracoContextFactory>();
        factoryMock.Setup(f => f.EnsureUmbracoContext()).Returns(referenceMock.Object);

        return (factoryMock, contextMock);
    }
}
