using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FluentValidation;
using QueueManagement.Api.DTOs.Common;

namespace QueueManagement.Api.Filters;

/// <summary>
/// Action filter for automatic FluentValidation
/// </summary>
public class FluentValidationActionFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public FluentValidationActionFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Execute the action filter
    /// </summary>
    /// <param name="context">Action executing context</param>
    /// <param name="next">Next action filter delegate</param>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Get all parameters that have validators
        var parameters = context.ActionDescriptor.Parameters;
        var validationFailures = new List<ValidationFailure>();

        foreach (var parameter in parameters)
        {
            if (context.ActionArguments.TryGetValue(parameter.Name, out var argument))
            {
                if (argument != null)
                {
                    var validatorType = typeof(IValidator<>).MakeGenericType(parameter.ParameterType);
                    var validator = _serviceProvider.GetService(validatorType);

                    if (validator != null)
                    {
                        var validationContext = new ValidationContext<object>(argument);
                        var validationResult = await ((IValidator)validator).ValidateAsync(validationContext);

                        if (!validationResult.IsValid)
                        {
                            validationFailures.AddRange(validationResult.Errors);
                        }
                    }
                }
            }
        }

        // If there are validation failures, return bad request
        if (validationFailures.Any())
        {
            var errors = validationFailures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(f => f.ErrorMessage).ToArray()
                );

            var apiResponse = new ApiResponse<object>
            {
                Success = false,
                Error = new ApiError
                {
                    Code = "VALIDATION_ERROR",
                    Message = "One or more validation errors occurred",
                    Details = validationFailures.Select(f => new ValidationError
                    {
                        Field = f.PropertyName,
                        Message = f.ErrorMessage
                    }).ToList()
                }
            };

            context.Result = new BadRequestObjectResult(apiResponse);
            return;
        }

        await next();
    }
}