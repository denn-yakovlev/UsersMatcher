using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UsersMatcher.Models
{
    public class Friends: ILastFmJsonResponse<User>
    { 
        [JsonProperty("user")]
        public List<User> Content { get; set; }

        [JsonProperty("@attr")]
        public Attributes Attributes{ get; set; }

    }
}
