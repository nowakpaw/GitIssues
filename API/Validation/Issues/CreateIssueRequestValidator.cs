using FluentValidation;
using Shared.Contracts.Requests.Issues;

namespace API.Validation.Issues;

public sealed class CreateIssueRequestValidator : BaseIssueRequestValidator<CreateIssueRequest>
{
    public CreateIssueRequestValidator()
    {
        RuleFor(r => r.Title)
            .NotEmpty()
            .WithMessage(string.Format(ValidationMessages.IsRequired, nameof(CreateIssueRequest.Title)));

        RuleFor(r => r.Description)
            .NotEmpty()
            .WithMessage(string.Format(ValidationMessages.IsRequired, nameof(CreateIssueRequest.Description)));
    }
}
