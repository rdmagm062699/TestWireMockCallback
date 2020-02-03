namespace AcceptanceTests
{
    using System.Collections.Generic;
    using WireMock.Server;

    public class Env
    {
        public WireMockServer mockServer { get; set; }
        public Dictionary<string, Item> eventualConsistenData { get; }
        public string consoleOutput { get; set; }

        public Env()
        {
            eventualConsistenData = new Dictionary<string, Item>();
        }
    }
}