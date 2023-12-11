using RestClientTask.Application.Models;
using RestClientTask.Factory;
using System.Net;
using System.Text.Json;

namespace RestClientTask
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var webRequestExample = RestClientFactory.CreateWebRequest().WithBaseUrl("https://openexchangerates.org").WithBasicAuth("user", "password").WithHeader("key", "value");
            var req = webRequestExample.CreateRequest("api/latest.json?app_id=fb2eb11772b84facbcfd4e252029e641").WithHeader("Accept", "application/json");
            var res = await req.GetAsync();
            if(res != null && res.StatusCode == HttpStatusCode.OK)
            {
                var entity = await JsonSerializer.DeserializeAsync<ExchangeRateListLogicModel>(await res.Content.ReadAsStreamAsync(), new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                Console.WriteLine($"WebReqeust result- {JsonSerializer.Serialize(entity)}");
            }
            else
            {
                string error = res != null ? await res.Content.ReadAsStringAsync() : "Internal server error";
                Console.WriteLine(error);
            }

            var httpClientExample = RestClientFactory.CreateDefault().WithBaseUrl("https://openexchangerates.org").WithBasicAuth("user", "password").WithHeader("key", "value");
            var request = httpClientExample.CreateRequest("api/latest.json?app_id=fb2eb11772b84facbcfd4e252029e641").WithHeader("Accept", "application/json");
            var result = await request.GetAsync();
            if (result != null && res.StatusCode == HttpStatusCode.OK)
            {
                var entity = await JsonSerializer.DeserializeAsync<ExchangeRateListLogicModel>(await result.Content.ReadAsStreamAsync(), new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                Console.WriteLine($"HttpClient result - {JsonSerializer.Serialize(entity)}");
            }
            else
            {
                string error = res != null ? await res.Content.ReadAsStringAsync() : "Internal server error";
                Console.WriteLine(error);
            }
        }
    }
}