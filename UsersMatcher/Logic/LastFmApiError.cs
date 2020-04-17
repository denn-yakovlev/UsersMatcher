using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMatcher.Logic
{
    public class LastFmApiError : Exception
    {
        public int StatusCode { get; set; }

        public string Reason { get; set; }
    }
}
