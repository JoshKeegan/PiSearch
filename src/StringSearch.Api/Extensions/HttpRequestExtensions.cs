using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace StringSearch.Api.Extensions
{
    public static class HttpRequestExtensions
    {
        public static IPAddress GetClientIp(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return request.Headers.TryGetValue("X-Forwarded-For", out StringValues strValsForwardedFor) &&
                   strValsForwardedFor.Count > 0 && IPAddress.TryParse(strValsForwardedFor[0], out IPAddress clientIp)
                ? clientIp
                : request.HttpContext.Connection.RemoteIpAddress;
        }
    }
}
