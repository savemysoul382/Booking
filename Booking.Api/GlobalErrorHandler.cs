using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace Booking.Api
{
    public class GlobalErrorHandler
    {
        private readonly RequestDelegate next;

        public GlobalErrorHandler(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.next(context: context);
            }
            catch (ValidationException validation_error)
            {
                await CreateResponse(context: context, status_code: HttpStatusCode.BadRequest, message: validation_error.Message);
            }
            catch (KeyNotFoundException error)
            {
                await CreateResponse(context: context, status_code: HttpStatusCode.NotFound, message: error.Message);
            }
            catch (Exception error)
            {
                await CreateResponse(context: context, status_code: HttpStatusCode.InternalServerError, message: error.Message);
            }
        }

        private async Task CreateResponse(HttpContext context,
            HttpStatusCode status_code = HttpStatusCode.InternalServerError,
            String message = "Internal error occurred")
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (Int32) status_code;
            String result = JsonSerializer.Serialize(new {message});
            await context.Response.WriteAsync(text: result);
        }
    }
}