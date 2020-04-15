using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMatcher.Models
{
    public class TopAlbums: ILastFmJsonResponse<Album>
    {
        [JsonProperty("album")]
        public List<Album> Content { get; set; }

        [JsonProperty("@attr")]
        public Attributes Attributes{ get; set; }
    }
}
