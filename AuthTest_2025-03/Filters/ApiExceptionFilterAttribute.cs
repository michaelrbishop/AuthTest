using AuthTest_2025_03.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;

namespace AuthTest_2025_03.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;
        // TODO : MRB Add logging
        public ApiExceptionFilterAttribute()
        {
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(ValidationException), HandleValidationException }, // 400 Bad Request
                { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException }, // 401 Unauthorized
                { typeof(NotFoundException), HandleNotFoundException }, // 404 Not found
                { typeof(InvalidOperationException), HandleInvalidOperationException }, // 409
                // 403 Forbidden ?
                
                // Add more exception handlers here
            };
        }

        public override void OnException(ExceptionContext context)
        {
            if (_exceptionHandlers.TryGetValue(context.Exception.GetType(), out var handler))
            {
                handler(context);
            }
            else
            {
                HandleUnknownException(context);
            }

            base.OnException(context);
        }

        private void HandleValidationException(ExceptionContext context)
        {
            var exception = context.Exception as ValidationException;
            var dataDictionary = BuildDataDictionary(exception);
            var problemDetails = new ProblemDetails
            {
                Title = "Validation error",
                Detail = exception?.Message,
                Status = StatusCodes.Status400BadRequest,
                Type = "https://httpstatues.com/400",
                Instance = context.HttpContext.Request.Path,
                Extensions = dataDictionary ?? [],
            };

            context.Result = new BadRequestObjectResult(problemDetails);
            context.ExceptionHandled = true;

        }

        private void HandleNotFoundException(ExceptionContext context)
        {
            var exception = context.Exception as NotFoundException;
            var dataDictionary = BuildDataDictionary(exception);
            var problemDetails = new ProblemDetails
            {
                Title = "Resource not found",
                Detail = exception?.Message,
                Status = StatusCodes.Status404NotFound,
                Type = "https://httpstatues.com/400",
                Instance = context.HttpContext.Request.Path,
                Extensions = dataDictionary ?? [],
            };

            context.Result = new NotFoundObjectResult(problemDetails);
            context.ExceptionHandled = true;

        }

        private void HandleUnauthorizedAccessException(ExceptionContext context)
        {
            var exception = context.Exception as UnauthorizedAccessException;
            var dataDictionary = BuildDataDictionary(exception);
            var problemDetails = new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = exception?.Message,
                Status = StatusCodes.Status401Unauthorized,
                Type = "https://httpstatues.com/400",
                Instance = context.HttpContext.Request.Path,
                Extensions = dataDictionary ?? [],
            };

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = StatusCodes.Status401Unauthorized,
            };
            context.ExceptionHandled = true;

        }

        public void HandleInvalidOperationException(ExceptionContext context)
        {
            var exception = context.Exception as InvalidOperationException;
            var dataDictionary = BuildDataDictionary(exception);
            var problemDetails = new ProblemDetails
            {
                Title = "Conflict",
                Detail = exception?.Message,
                Status = StatusCodes.Status409Conflict,
                Type = "https://httpstatues.com/409",
                Instance = context.HttpContext.Request.Path,
                Extensions = dataDictionary ?? [],
            };

            context.Result = new ConflictObjectResult(problemDetails);
            context.ExceptionHandled = true;
        }

        private void HandleUnknownException(ExceptionContext context)
        {
            var exception = context.Exception as ValidationException;
            var dataDictionary = BuildDataDictionary(exception);
            var problemDetails = new ProblemDetails
            {
                Title = "An error occurred while processing your request",
                Detail = exception?.Message,
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://httpstatues.com/400",
                Instance = context.HttpContext.Request.Path,
                Extensions = dataDictionary ?? [],
            };

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = StatusCodes.Status500InternalServerError,
            };
            context.ExceptionHandled = true;

        }

        private static Dictionary<string, object?>? BuildDataDictionary(Exception? exception)
        {
            // TODO : MRB Consider just logging this and not sending it back to the user.
            // Why would they need any of this info?
            if (exception != null)
            {
                var dataDictionary = new Dictionary<string, object?>();
                foreach (var key in exception.Data.Keys)
                {
                    if (key != null && exception.Data[key] != null)
                    {
                        dataDictionary.Add(key.ToString()!, exception.Data[key]);
                    }
                }

                return dataDictionary;
            }

            return null;
        }
    }
}
