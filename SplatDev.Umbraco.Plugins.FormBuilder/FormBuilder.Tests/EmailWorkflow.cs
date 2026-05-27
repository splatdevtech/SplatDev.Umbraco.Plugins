using FormBuilder.Extension.Entities;
using FormBuilder.Extension.Interfaces;
using FormBuilder.Extension.Workflows;

using Moq;

using Umbraco.Cms.Tests.Common.Testing;
using Umbraco.Cms.Tests.Integration.Testing;

namespace SplatDev.Umbraco.Plugins.FormBuilder
{
    [TestFixture]
    [UmbracoTest(Database = UmbracoTestOptions.Database.NewSchemaPerTest)]
    public class IntegrationTests : UmbracoIntegrationTest
    {

        [Test]
        public async Task EmailWorkflow_Sends_Valid_Email()
        {
            var mockEmail = new Mock<IEmailService>();
            var mockRepository = new Mock<IFormRepository>();

            // Setup repository to return a valid form with workflow settings
            mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Form
                {
                    Id = 1,
                    Name = "Test Form",
                    Workflows = [] // Add a workflow if your workflow expects it
                });

            var workflow = new EmailWorkflow(mockRepository.Object, mockEmail.Object);

            await workflow.ExecuteAsync(new WorkflowExecutionContext(1, new Guid("c914e681-0e35-4fd7-879a-ca3d71c9ebe4")));

            mockEmail.Verify(e => e.SendAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>() // <-- Specify the optional 'from' parameter
            ), Times.Once);
        }
    }

}
