using Educational_Platform.Domain.Exceptions;
using Microsoft.Extensions.Primitives;

namespace Educational_Platform.API.Middlewares
{
	public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> log)
	{
		public async Task Invoke(HttpContext httpContext)
		{
			try
			{
				await next.Invoke(httpContext);
			}
			catch (Exception ex)
			{
				HandleException(httpContext, ex);
				log.LogError("Error:{Message},{Source},{Inner}", ex.Message, ex.Source, ex.InnerException);
			}
		}
		private void HandleException(HttpContext httpContext, Exception exception)
		{
			var res = httpContext.Response;

			if (exception is UnauthorizedAccessException)
				res.StatusCode = StatusCodes.Status403Forbidden;

			else if (exception is NotExistException)
				res.StatusCode = StatusCodes.Status404NotFound;

			else if (exception is DataBaseDownException)
				res.StatusCode = StatusCodes.Status500InternalServerError;

			if (exception.Data.Contains(ExceptionKeys.StatusCodeKey))
			{
				res.StatusCode = Convert.ToInt32(exception.Data[ExceptionKeys.StatusCodeKey]);
			}

			var message = new StringValues(exception.Message);

			httpContext.Response.Headers.TryAdd("Error-Message", message);
		}
	}
}
