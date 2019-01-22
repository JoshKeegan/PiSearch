using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using StringSearch.Api.Contracts;
using StringSearch.Api.Contracts.Errors;

namespace StringSearch.Api.Mvc.Middleware
{
    public class ExceptionRequestHandler
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;
        private readonly IHostingEnvironment hostingEnvironment;

        public ExceptionRequestHandler(RequestDelegate next, ILogger logger, IHostingEnvironment hostingEnvironment)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                Guid errorId = Guid.NewGuid();
                string requestBody = await getRequestBody(context.Request);
                logger.Error(e,
                    "Error ID {@errorId}. Query String {@queryString}. Request Body {@requestBody}.",
                    errorId, context.Request.QueryString, requestBody);

                // Construct an error. If in development, add additional info
                Error error;
                if (hostingEnvironment.IsDevelopment())
                {
                    error = new DevelopmentError()
                    {
                        Exception = e
                    };
                }
                else
                {
                    error = new Error();
                }
                error.Message = "An unexpected error has occurred";
                error.Id = errorId;

                ErrorResponse response = new ErrorResponse() { Error = error };
                string strResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings()
                {
                    // An exception may have circular references... Ignore and just serialise what we can.
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(strResponse);
            }
        }

        private static async Task<string> getRequestBody(HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.ContentLength == null)
            {
                return "";
            }

            long prevPos = request.Body.Position;
            request.Body.Position = 0;
            byte[] buffer = new byte[request.ContentLength.Value];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            request.Body.Position = prevPos;
            return Encoding.UTF8.GetString(buffer);
        }
    }
}
