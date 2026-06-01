using Moq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;

namespace SplatDev.Umbraco.Tools.Tests;

public abstract class UmbracoTestBase
{
    protected Mock<IContentService> ContentServiceMock { get; } = new();
    protected Mock<IMemberService> MemberServiceMock { get; } = new();
    protected Mock<IPublishedContentCache> PublishedContentCacheMock { get; } = new();
    protected Mock<IUmbracoContextFactory> UmbracoContextFactoryMock { get; } = new();
    protected Mock<IUmbracoContext> UmbracoContextMock { get; } = new();

    protected UmbracoTestBase()
    {
        UmbracoContextMock
            .Setup(c => c.Content)
            .Returns(PublishedContentCacheMock.Object);

        // UmbracoContextReference is a concrete class (not an interface) in both
        // Umbraco 13 and 17, with non-virtual properties — create a real instance.
        var contextAccessorMock = new Mock<IUmbracoContextAccessor>();
        var contextReference = new global::Umbraco.Cms.Core.UmbracoContextReference(
            UmbracoContextMock.Object, true, contextAccessorMock.Object);

        UmbracoContextFactoryMock
            .Setup(f => f.EnsureUmbracoContext())
            .Returns(contextReference);
    }
}
