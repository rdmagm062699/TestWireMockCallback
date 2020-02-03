using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using WireMock.Server;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Types;
using WireMock.Util;

namespace TestWireMock
{
    class Item
    {
        public string Value { get; set; }
        public long AvailableAtSeconds { get; set; }
    }

    class Program
    {
        private static Dictionary<string, Item> eventualConsistenData = new Dictionary<string, Item>();

        static void Main(string[] args)
        {
            var server = WireMockServer.Start();

            StubSomething(server, "one", "The number is 1");
            CallIt(server, "one");

            server.Stop();
        }

        private static void StubSomething(WireMockServer server, string param, string body)
        {
            eventualConsistenData.Add(
                param, 
                new Item { Value = body, AvailableAtSeconds = DateTimeOffset.Now.ToUnixTimeSeconds() + 5 }
            );

            server
                .Given(
                    Request.Create().WithPath("/something").UsingGet()
                )
                .RespondWith(
                    Response.Create().WithCallback(req => BuildResponse(req))
                );
        }

        private static WireMock.ResponseMessage BuildResponse(WireMock.RequestMessage request)
        {
            var parmValue = request.GetParameter("num").FirstOrDefault();
            var data = eventualConsistenData[parmValue];
            var currentSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();

            if (currentSeconds >= data.AvailableAtSeconds)
            {
                return new WireMock.ResponseMessage {
                    StatusCode = 200,
                    BodyData = new BodyData { DetectedBodyType = BodyType.String, BodyAsString = data.Value }
                };
            }
            else
            {
                return new WireMock.ResponseMessage {
                    StatusCode = 404,
                    BodyData = new BodyData { DetectedBodyType = BodyType.String, BodyAsString = "Data not found...." }
                };
            }
        }

        private static void CallIt(WireMockServer server, string param)
        {
            var url = server.Urls[0];
            var client = new HttpClient();
            var stop = false;

            while (stop == false)
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, $"{url}/something?num={param}"))
                {
                    var response = client.SendAsync(request).Result;
                    var content = response.Content.ReadAsStringAsync().Result;

                    Console.Out.WriteLine("------------------------------------------");
                    Console.Out.WriteLine($"Status Code: {response.StatusCode}");
                    Console.Out.WriteLine($"Content: {content}");

                    try
                    {
                        response.EnsureSuccessStatusCode();
                        stop = true;
                    }
                    catch
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            }
        }
    }
}
