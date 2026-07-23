namespace SplatDev.Umbraco.Plugins.FormBuilder
{
    [SetUpFixture]
    public class CustomGlobalSetupTeardown
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private static GlobalSetupTeardown _setupTearDown;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        [OneTimeSetUp]
        public void SetUp()
        {
            _setupTearDown = new GlobalSetupTeardown();
            _setupTearDown.SetUp();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _setupTearDown.TearDown();
        }
    }
}
