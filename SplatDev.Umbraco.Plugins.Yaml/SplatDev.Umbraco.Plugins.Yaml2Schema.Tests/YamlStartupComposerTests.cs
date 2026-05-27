using Xunit;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Composers;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Services;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Tests
{
    public class YamlStartupComposerTests
    {
        [Fact]
        public void YamlStartupComposer_ShouldRegisterServices()
        {
            // Arrange
            var mockBuilder = new Mock<IUmbracoBuilder>();
            var serviceCollection = new ServiceCollection();
            mockBuilder.Setup(x => x.Services).Returns(serviceCollection);

            var composer = new YamlStartupComposer();

            // Act
            composer.Compose(mockBuilder.Object);

            // Assert - verify all service types are registered in the DI container
            // (not resolved, since Umbraco service dependencies are not available in this test context)
            Assert.Contains(serviceCollection, sd => sd.ServiceType == typeof(YamlParser));
            Assert.Contains(serviceCollection, sd => sd.ServiceType == typeof(DataTypeCreator));
            Assert.Contains(serviceCollection, sd => sd.ServiceType == typeof(DocumentTypeCreator));
            Assert.Contains(serviceCollection, sd => sd.ServiceType == typeof(MediaTypeCreator));
            Assert.Contains(serviceCollection, sd => sd.ServiceType == typeof(TemplateCreator));
            Assert.Contains(serviceCollection, sd => sd.ServiceType == typeof(ContentCreator));
            Assert.Contains(serviceCollection, sd => sd.ServiceType == typeof(MediaCreator));
            Assert.Contains(serviceCollection, sd => sd.ServiceType == typeof(StaticAssetCreator));
            Assert.Contains(serviceCollection, sd => sd.ServiceType == typeof(LanguageCreator));
            Assert.Contains(serviceCollection, sd => sd.ServiceType == typeof(DictionaryCreator));
            Assert.Contains(serviceCollection, sd => sd.ServiceType == typeof(MemberCreator));
            Assert.Contains(serviceCollection, sd => sd.ServiceType == typeof(UserCreator));
            Assert.Contains(serviceCollection, sd => sd.ServiceType == typeof(PackageValidator));
            Assert.Contains(serviceCollection, sd => sd.ServiceType == typeof(PropertyEditorCreator));
            // Note: AddNotificationAsyncHandler is an extension method and cannot be verified via Moq
        }
    }
}
