using System.Collections.Generic;

namespace ReservoirDevs.Logging.Models
{
    public class HttpResponseModel
    {
        public string Body { get; set; }

        public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }

        public override string ToString()
        {
            return $"Body: {Body}";
        }
    }
}