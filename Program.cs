using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace openbanking
{

    public class Program
    {
        private static readonly HttpClient client = new HttpClient();

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
            ProcessRepositories().Wait();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
        
        private static async Task ProcessRepositories()
        {
            var serializer = new DataContractJsonSerializer(typeof(List<repo>));


            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            
            var streamTask = client.GetStreamAsync("https://api.github.com/orgs/dotnet/repos");
            var repositories = serializer.ReadObject(await streamTask) as List<repo>;
            //https://api.nordeaopenbanking.com/v1/authentication?client_id={CLIENT_ID}&redirect_uri=http://httpbin.org/get&X-Response-Scenarios=AuthenticationSkipUI&state=

            foreach (var repo in repositories)
                Console.WriteLine(repo.id);
        }
    }
}
