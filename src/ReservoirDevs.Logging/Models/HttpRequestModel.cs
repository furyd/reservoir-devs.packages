namespace ReservoirDevs.Logging.Models
{
    public class HttpRequestModel : HttpResponseModel
    {
        public string Scheme { get; set; }

        public string Host { get; set; }

        public string Path { get; set; }

        public string Querystring { get; set; }

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}