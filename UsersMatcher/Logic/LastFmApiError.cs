using System;
using UsersMatcher.Models;

namespace UsersMatcher.Logic
{
    public class LastFmApiError : Exception
    {
        public LastFmErrorCode Code { get;  }

        public string UserName { get; set; }

        public override string Message { get; }

        public LastFmApiError(LastFmErrorCode code, string message)
        {
            Code = code;
            Message = message;
        }
    }

    public enum LastFmErrorCode
    {
        InvalidService = 2,
        InvalidMethod,
        AuthenticationFailed,
        InvalidFormat,
        InvalidParameters,
        InvalidResource,
        OperationFailed,
        InvalidSessionKey,
        InvalidApiKey,
        ServiceOffline,
        InvalidMethodSig = 13,
        TemporaryError = 16,
        SuspendedApiKey = 26,
        RateLimitExceeded = 29
    }
}
