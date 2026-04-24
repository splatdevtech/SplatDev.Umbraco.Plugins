using Moq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;

namespace SplatDev.Umbraco.Tools.Tests;

public abstract class UmbracoTestBase
{
    protected Mock<IContentService> ContentServiceMock { get; } = new();
    protected Mock<IMemberService> MemberServiceMock { get; } = new();
    protected Mock<IPublishedContentCache> PublishedContentCacheMock { get; } = new();
    protected Mock<IUmbracoContextFactory> UmbracoContextFactoryMock { get; } = new();
    protected Mock<IUmbracoContextReference> UmbracoContextReferenceMock { get; } = new();
    protected Mock<IUmbracoContext> UmbracoContextMock { get; } = new();

    protected UmbracoTestBase()
    {
        UmbracoContextMock
            .Setup(c => c.Content)
            .Returns(PublishedContentCacheMock.Object);

        UmbracoContextReferenceMock
            .Setup(r => r.UmbracoContext)
            .Returns(UmbracoContextMock.Object);

        UmbracoContextFactoryMock
            .Setup(f => f.EnsureUmbracoContext())
            .Returns(UmbracoContextReferenceMock.Object);
    }
}
