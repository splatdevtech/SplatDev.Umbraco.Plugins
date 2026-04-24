namespace SplatDev.Umbraco.Plugins.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using UmbracoCms.CodeFirst.Helpers;
    [TestClass]
    public class IISHelpers_Tests
    {
        [TestMethod]
        public void RestartAppPool()
        {
            // Arrange

            // Act
            IISHelpers.RestartApplicationPool();

            // Assert
            Assert.IsTrue(true);
        }
    }
}
