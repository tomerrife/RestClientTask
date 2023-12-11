namespace RestClientTask.Interfaces
{
    public interface IRestClient 
    {
        IRestClient WithBaseUrl(string baseUrl);
        IRestClient WithBasicAuth(string user, string password);
        IRestClient WithHeader(string key, string value);

        IRestRequest CreateRequest(string endpoint);
    }
}
