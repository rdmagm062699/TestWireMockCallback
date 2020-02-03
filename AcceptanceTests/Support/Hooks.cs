namespace AcceptanceTests
{
    using TechTalk.SpecFlow;
    using WireMock.Server;

    public static class Hooks
    {
        public static Env Env { get; } = new Env();

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            Env.mockServer = WireMockServer.Start();
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            Env.mockServer.Stop();
        }
    }
}