using RestClientTask.Domain;
using RestClientTask.Interfaces;
using System.Net;
using System.Text;

namespace RestClientTask.Framework.HTTP.HttpClient
{
    public class RestRequestHttpClient : IRestRequest
    {
        private readonly RestClientConfig RestClientConfig;
        private readonly RestRequestConfig RestRequestConfig;
        public RestRequestHttpClient(RestClientConfig restClientConfig, RestRequestConfig restRequestConfig)
        {
            RestClientConfig = restClientConfig;
            RestRequestConfig = restRequestConfig;
        }
        public IRestRequest WithBasicAuth(string user, string password)
        {
            RestRequestConfig.BasicAuthUser = user;
            RestRequestConfig.BasicAuthPassword = password;
            return this;
        }

        public IRestRequest WithHeader(string key, string value)
        {
            if (!RestRequestConfig.Headers.ContainsKey(key))
            {
                RestRequestConfig.Headers.Add(key, value);
            }
            else
            {
                RestRequestConfig.Headers[key] = value;
            }
            return this;
        }
        public async Task<HttpResponseMessage> GetAsync()
        {
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                httpClient.BaseAddress = new Uri(RestClientConfig.BaseUrl);

                InitialAuthorization(httpClient);
                InitialHeaders(httpClient);

                var response = await httpClient.GetAsync($"/{RestRequestConfig.Endpoint}");
                return response;
            }
        }

        public async Task<HttpResponseMessage> PostAsync(string content)
        {
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                httpClient.BaseAddress = new Uri(RestClientConfig.BaseUrl);

                InitialAuthorization(httpClient);
                InitialHeaders(httpClient);

                var contentData = new StringContent(content, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"/{RestRequestConfig.Endpoint}", contentData);
                return response;
            }
        }
        private void InitialHeaders(System.Net.Http.HttpClient httpClient)
        {
            var headers = RestClientConfig.Headers
                                          .Concat(RestRequestConfig.Headers
                                          .Where(x => !RestClientConfig.Headers.Keys.Contains(x.Key)))
                                          .ToDictionary(x => x.Key, x => x.Value);

            foreach (var header in headers)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        private void InitialAuthorization(System.Net.Http.HttpClient httpClient)
        {
            if (!string.IsNullOrEmpty(RestClientConfig.BasicAuthUser) && !string.IsNullOrEmpty(RestClientConfig.BasicAuthPassword))
            {
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{RestClientConfig.BasicAuthUser}:{RestClientConfig.BasicAuthPassword}"));
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
            }
            else if (!string.IsNullOrEmpty(RestRequestConfig.BasicAuthUser) && !string.IsNullOrEmpty(RestRequestConfig.BasicAuthPassword))
            {
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{RestRequestConfig.BasicAuthUser}:{RestRequestConfig.BasicAuthPassword}"));
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
            }
        }
    }
}
