namespace ReservoirDevs.Logging.Models
{
    public class HttpRequestModel : HttpResponseModel
    {
        public string Scheme { get; set; }

        public string Host { get; set; }

        public string Path { get; set; }

        public string Querystring { get; set; }
    }
}