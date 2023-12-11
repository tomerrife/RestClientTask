namespace RestClientTask.Interfaces
{
    public interface IRestRequest
    {
        IRestRequest WithBasicAuth(string user, string password);
        IRestRequest WithHeader(string key, string value);

        Task<HttpResponseMessage> GetAsync();
        Task<HttpResponseMessage> PostAsync(string content);
    }
}
