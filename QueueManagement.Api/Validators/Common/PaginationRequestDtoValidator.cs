using FluentValidation;
using QueueManagement.Api.DTOs.Common;

namespace QueueManagement.Api.Validators.Common;

/// <summary>
/// Validator for PaginationRequestDto
/// </summary>
public class PaginationRequestDtoValidator : AbstractValidator<PaginationRequestDto>
{
    public PaginationRequestDtoValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 1000)
            .WithMessage("Page size must be between 1 and 1000");

        RuleFor(x => x.SortDirection)
            .Must(x => string.IsNullOrEmpty(x) || x.ToLower() == "asc" || x.ToLower() == "desc")
            .WithMessage("Sort direction must be 'asc' or 'desc'");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.SearchTerm))
            .WithMessage("Search term cannot exceed 200 characters");

        RuleFor(x => x.Filters)
            .Must(x => x == null || x.Count <= 20)
            .When(x => x.Filters != null)
            .WithMessage("Cannot apply more than 20 filters at once");
    }
}