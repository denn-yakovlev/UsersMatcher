using Newtonsoft.Json;

namespace UsersMatcher.Models
{
    public class Attributes
    {
        [JsonProperty("user")]
        public string UserName { get; set; }

        public int Page { get; set; }

        public int PerPage { get; set; }

        public int Total { get; set; }

        public int TotalPages { get; set; }
    }
}