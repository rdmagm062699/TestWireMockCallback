namespace AcceptanceTests
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using FluentAssertions;
    using TechTalk.SpecFlow;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using WireMock.Server;
    using WireMock.Types;
    using WireMock.Util;

    [Binding]
    public class Something
    {
        [Given(@"The mock is set up")]
        public void GivenTheMockIsSetUp()
        {
            Hooks.Env.eventualConsistenData.Add(
                "myParamValue",
                new Item { Value = "This body is expected", AvailableAtSeconds = DateTimeOffset.Now.ToUnixTimeSeconds() + 5 }
            );

            if (Hooks.Env.mockServer == null)
            {
                Hooks.Env.mockServer = WireMockServer.Start();
            }

            Hooks.Env.mockServer
                .Given(
                    Request.Create().WithPath("/something").UsingGet()
                )
                .RespondWith(
                    Response.Create().WithCallback(req => BuildResponse(req))
                );
        }

        private WireMock.ResponseMessage BuildResponse(WireMock.RequestMessage request)
        {
            var parmValue = request.GetParameter("myParam").FirstOrDefault();
            var data = Hooks.Env.eventualConsistenData[parmValue];
            var currentSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();

            if (currentSeconds >= data.AvailableAtSeconds)
            {
                return new WireMock.ResponseMessage
                {
                    StatusCode = 200,
                    BodyData = new BodyData { DetectedBodyType = BodyType.String, BodyAsString = data.Value }
                };
            }
            else
            {
                return new WireMock.ResponseMessage
                {
                    StatusCode = 404,
                    BodyData = new BodyData { DetectedBodyType = BodyType.String, BodyAsString = "Data not found...." }
                };
            }
        }

        [When(@"The program is run")]
        public void TheProgramIsRun()
        {
            var url = $"{Hooks.Env.mockServer.Urls[0]}/something?myParam=myParamValue";
            string output;

            var file = new System.Diagnostics.StackTrace(true).GetFrame(0).GetFileName();
            var fileDir = new System.IO.FileInfo(file).Directory;

            using (var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"run --project {fileDir}/../../src/TestWireMockCallback.csproj \"{url}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            })
            {
                Console.Out.WriteLine($"Run project {process.StartInfo.Arguments}");
                process.Start();
                output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
                process.WaitForExit();
            }

            Hooks.Env.consoleOutput = output;
        }

        [Then(@"This is the result")]
        public void ThisIsTheResult()
        {
            Hooks.Env.consoleOutput.Should().Contain("This body is expected");
        }
    }
}