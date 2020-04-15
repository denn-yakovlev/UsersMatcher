using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UsersMatcher.Models
{
    public class User
    {
        public string Name { get; set; }

        [JsonProperty("realname")]
        public string RealName { get; set; }

        public string Url { get; set; }

        [JsonProperty("playcount")]
        public int PlayCount { get; set; }
    }
}
