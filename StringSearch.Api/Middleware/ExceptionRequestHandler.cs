using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using StringSearch.Api.ViewModels;

namespace StringSearch.Api.Middleware
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
                logger.Error(e, "Error ID {@errorId}. Query String {@queryString}. Request Body {@requestBody}. Message: " + e.Message, errorId,
                    context.Request.QueryString.ToString(), requestBody);

                // Construct an error. If in development, add additional info
                VmError error;
                if (hostingEnvironment.IsDevelopment())
                {
                    error = new VmDevelopmentError()
                    {
                        Exception = e
                    };
                }
                else
                {
                    error = new VmError();
                }
                error.Message = "An unexpected error has occurred";
                error.Id = errorId;

                VmErrorResponse response = new VmErrorResponse() { Error = error };
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
