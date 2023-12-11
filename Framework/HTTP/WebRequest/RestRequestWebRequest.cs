using RestClientTask.Domain;
using RestClientTask.Interfaces;
using System.Net;
using System.Text;

namespace RestClientTask.Framework.HTTP.WebRequest
{
    public class RestRequestWebRequest : IRestRequest
    {
        private readonly RestClientConfig RestClientConfig;
        private readonly RestRequestConfig RestRequestConfig;

        public RestRequestWebRequest(RestClientConfig restClientConfig, RestRequestConfig restRequestConfig)
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
            var url = $"{RestClientConfig.BaseUrl}/{RestRequestConfig.Endpoint}";
            var request = (HttpWebRequest)System.Net.WebRequest.Create(url);

            InitialAuthorization(request);
            InitialHeaders(request);
            try
            {
                using (var response = await request.GetResponseAsync())
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = await streamReader.ReadToEndAsync();
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(result) };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("An unexpected error occurred while processing the request") };
            }
        }

        public async Task<HttpResponseMessage> PostAsync(string content)
        {
            var url = $"{RestClientConfig.BaseUrl}/{RestRequestConfig.Endpoint}";
            var request = (HttpWebRequest)System.Net.WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";

            InitialAuthorization(request);
            InitialHeaders(request);
            try
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    await streamWriter.WriteAsync(content);
                    await streamWriter.FlushAsync();
                }

                using (var response = await request.GetResponseAsync())
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = await streamReader.ReadToEndAsync();
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(result) };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("An unexpected error occurred while processing the request") };
            }
        }
        private void InitialHeaders(HttpWebRequest request)
        {
            var headers = RestClientConfig.Headers
                                          .Concat(RestRequestConfig.Headers
                                          .Where(x => !RestClientConfig.Headers.Keys.Contains(x.Key)))
                                          .ToDictionary(x => x.Key, x => x.Value);

            foreach (var header in headers)
            {
                request.Headers[header.Key] = header.Value;
            }
        }

        private void InitialAuthorization(HttpWebRequest request)
        {
            if (!string.IsNullOrEmpty(RestClientConfig.BasicAuthUser) && !string.IsNullOrEmpty(RestClientConfig.BasicAuthPassword))
            {
                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{RestClientConfig.BasicAuthUser}:{RestClientConfig.BasicAuthPassword}"));
                request.Headers[HttpRequestHeader.Authorization] = $"Basic {credentials}";
            }
            else if (!string.IsNullOrEmpty(RestRequestConfig.BasicAuthUser) && !string.IsNullOrEmpty(RestRequestConfig.BasicAuthPassword))
            {
                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{RestRequestConfig.BasicAuthUser}:{RestRequestConfig.BasicAuthPassword}"));
                request.Headers[HttpRequestHeader.Authorization] = $"Basic {credentials}";
            }
        }
    }
}
