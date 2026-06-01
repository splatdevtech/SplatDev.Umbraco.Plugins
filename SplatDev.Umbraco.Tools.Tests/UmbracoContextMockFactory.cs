using Moq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Web;

namespace SplatDev.Umbraco.Tools.Tests;

public static class UmbracoContextMockFactory
{
    public static (Mock<IUmbracoContextFactory> Factory, Mock<IUmbracoContext> Context) Create(
        Action<Mock<IPublishedContentCache>>? configureCache = null)
    {
        var cacheMock = new Mock<IPublishedContentCache>();
        configureCache?.Invoke(cacheMock);

        var contextMock = new Mock<IUmbracoContext>();
        contextMock.Setup(c => c.Content).Returns(cacheMock.Object);

        var factoryMock = new Mock<IUmbracoContextFactory>();

        // UmbracoContextReference is a concrete class (not an interface) in both
        // Umbraco 13 and 17, with non-virtual properties — create a real instance.
        var contextAccessorMock = new Mock<IUmbracoContextAccessor>();
        var reference = new global::Umbraco.Cms.Core.UmbracoContextReference(
            contextMock.Object, true, contextAccessorMock.Object);
        factoryMock.Setup(f => f.EnsureUmbracoContext()).Returns(reference);

        return (factoryMock, contextMock);
    }
}
