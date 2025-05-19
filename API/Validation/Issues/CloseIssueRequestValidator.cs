using FluentValidation;
using Shared.Contracts.Requests.Issues;

namespace API.Validation.Issues;

public sealed class CloseIssueRequestValidator : BaseIssueRequestValidator<CloseIssueRequest>
{
    public CloseIssueRequestValidator()
    {
        RuleFor(r => r.Id)
            .GreaterThan(0)
            .WithMessage(string.Format(ValidationMessages.IsRequired, nameof(CloseIssueRequest.Id)));
    }
}