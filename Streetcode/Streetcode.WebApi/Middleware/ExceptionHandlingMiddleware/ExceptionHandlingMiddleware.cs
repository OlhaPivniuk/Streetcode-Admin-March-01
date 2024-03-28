using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Streetcode.WebApi.Middleware.ExceptionHandlingMiddleware;

public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var traceId = Guid.NewGuid();
        LogInfoAboutError(traceId, ex.Message, ex.StackTrace);

        var problemDetails = new ProblemDetails();

        switch (ex)
        {
            case ValidationException validationException:
                {
                    WriteInfoAboutErrorToProblemDetails(
                        problemDetails,
                        "Validation error",
                        StatusCodes.Status400BadRequest,
                        context.Request.Path,
                        $"One or more validation errors occured, traceID: {traceId}");

                    context.Response.StatusCode = StatusCodes.Status400BadRequest;

                    if (validationException.Errors is not null)
                    {
                        problemDetails.Extensions["errors"] = validationException.Errors;
                    }

                    break;
                }

            default:
                {
                    WriteInfoAboutErrorToProblemDetails(
                        problemDetails,
                        "Internal Server Error",
                        StatusCodes.Status500InternalServerError,
                        context.Request.Path,
                        $"Internal server error occured, traceID: {traceId}");

                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                    break;
                }
        }
    }

    private static void WriteInfoAboutErrorToProblemDetails(
        ProblemDetails problemDetails,
        string errorTitle,
        int errorStatusCode,
        string errorInstance,
        string errorDetail)
    {
        problemDetails.Title = errorTitle;
        problemDetails.Status = errorStatusCode;
        problemDetails.Instance = errorInstance;
        problemDetails.Detail = errorDetail;
    }

    private void LogInfoAboutError(Guid traceId, string message, string? stackTrace)
    {
        _logger.LogError($"Error occure while processing the request, TraceID: {traceId}, Message: {message}, StackTrace: {stackTrace}");
    }
}
