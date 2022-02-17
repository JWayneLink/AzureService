// This code requires the Nuget package Microsoft.AspNet.WebApi.Client to be installed.
// Instructions for doing this in Visual Studio:
// Tools -> Nuget Package Manager -> Package Manager Console
// Install-Package Newtonsoft.Json

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CallRequestResponseService
{
    class Program
    {
        static void Main(string[] args)
        {
            InvokeRequestResponseService().Wait();
        }

        static async Task InvokeRequestResponseService()
        {
            var handler = new HttpClientHandler()
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                        (httpRequestMessage, cert, cetChain, policyErrors) => { return true; }
            };
            using (var client = new HttpClient(handler))
            {
                // Request data goes here
                List<double> d1 = new List<double>();
                d1.Add(2);
                d1.Add(180);
                d1.Add(74);
                d1.Add(24);
                d1.Add(21);
                d1.Add(23.9091702);
                d1.Add(1.488172308);
                d1.Add(22);

                List<double> d2 = new List<double>();
                d2.Add(2);
                d2.Add(148);
                d2.Add(58);
                d2.Add(11);
                d2.Add(179);
                d2.Add(39.19207553);
                d2.Add(0.160829008);
                d2.Add(45);
                var post = new List<List<double>>();
                post.Add(d1);
                post.Add(d2);

                // Request data goes here
                var scoreRequest = new Dictionary<string, List<List<double>>>();
                scoreRequest.Add("data", post);

                const string apiKey = ""; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.BaseAddress = new Uri("http://05c4aa0d-2692-4af7-a66c-0204d19303a1.japaneast.azurecontainer.io/score");

                // WARNING: The 'await' statement below can result in a deadlock
                // if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false)
                // so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)

                //var requestString = JsonConvert.SerializeObject(scoreRequest);
                var requestString = JsonConvert.SerializeObject(scoreRequest);
                var content = new StringContent(requestString);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync("", content);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var MLresponse= JsonConvert.DeserializeObject(result).ToString();
                    var a=MLresponse.Trim('[', ']').Split(",").Select(x => x.Trim('"')).ToArray();
                    Console.WriteLine("Result: {0}", result);
                }
                else
                {
                    Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                    // Print the headers - they include the requert ID and the timestamp,
                    // which are useful for debugging the failure
                    Console.WriteLine(response.Headers.ToString());

                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                }
            }
        }
    }
}