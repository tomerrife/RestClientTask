namespace RestClientTask.Domain
{
    public class BaseRestConfig
    {
        public string BasicAuthUser { get; set; }
        public string BasicAuthPassword { get; set; }
        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();
    }
}
