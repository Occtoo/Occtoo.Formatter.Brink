using Newtonsoft.Json;

namespace Occtoo.Formatter.Brink.Models
{
    public class ErrorResponse
    {
        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
