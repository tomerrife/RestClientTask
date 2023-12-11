using RestClientTask.Domain;
using RestClientTask.Interfaces;

namespace RestClientTask.Framework.HTTP.HttpClient
{
    public class RestClientHttpClient : IRestClient
    {
        private readonly RestClientConfig Config = new RestClientConfig();

        public IRestRequest CreateRequest(string endpoint)
        {
            return new RestRequestHttpClient(Config, new RestRequestConfig { Endpoint = endpoint });
        }

        public IRestClient WithBaseUrl(string baseUrl)
        {
            Config.BaseUrl = baseUrl;
            return this;
        }

        public IRestClient WithBasicAuth(string user, string password)
        {
            Config.BasicAuthUser = user;
            Config.BasicAuthPassword = password;
            return this;
        }

        public IRestClient WithHeader(string key, string value)
        {
            if (!Config.Headers.ContainsKey(key))
            {
                Config.Headers.Add(key, value);
            }
            else
            {
                Config.Headers[key] = value;
            }
            return this;
        }
    }
}
