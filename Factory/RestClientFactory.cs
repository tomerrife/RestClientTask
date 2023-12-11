using RestClientTask.Framework.HTTP.HttpClient;
using RestClientTask.Framework.HTTP.WebRequest;
using RestClientTask.Interfaces;

namespace RestClientTask.Factory
{
    public static class RestClientFactory
    {
        public static IRestClient CreateDefault()
        {
            return CreateHttpClient();
        }

        public static IRestClient CreateWebRequest()
        {
            return new RestClientWebRequest();
        }

        public static IRestClient CreateHttpClient()
        {
            return new RestClientHttpClient();
        }
    }
}
