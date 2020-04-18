using System;
using UsersMatcher.Models;

namespace UsersMatcher.Logic
{
    public class LastFmApiError : Exception
    {
        public int StatusCode { get; set; }

        public string Reason { get; set; }

        public string UserName { get; set; }

        public override string Message
        {
            get
            {
                switch (StatusCode)
                {
                    case 404: return $"It seems like user {UserName} doesn't exist";
                    case 403: return $"Too many requests/Invalid auth token";
                    default: return "";
                }
            }
        }
    }
}
