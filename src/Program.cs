using System;
using System.Net.Http;

namespace TestWireMock
{
    class Item
    {
        public string Value { get; set; }
        public long AvailableAtSeconds { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var url = args[0];
            CallIt(url);
        }

        private static void CallIt(string url)
        {
            var client = new HttpClient();
            var stop = false;

            while (stop == false)
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
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
