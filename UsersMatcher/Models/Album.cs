using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UsersMatcher.Models
{
    public class Album
    {
        public string Name { get; set; }

        public Artist Artist{ get; set; }
   
        public string Url { get; set; }

        public string Mbid { get; set; }
    }
}
