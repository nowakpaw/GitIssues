using FluentValidation;
using Shared.Contracts.Requests.Issues;

namespace API.Validation.Issues;

public sealed class UpdateIssueRequestValidator : BaseIssueRequestValidator<UpdateIssueRequest>
{
    public UpdateIssueRequestValidator()
    {
        RuleFor(r => r.Id)
            .GreaterThan(0)
            .WithMessage(string.Format(ValidationMessages.IsRequired, nameof(UpdateIssueRequest.Id)));

        RuleFor(r => r.Title)
            .NotEmpty()
            .WithMessage(string.Format(ValidationMessages.IsRequired, nameof(UpdateIssueRequest.Title)));

        RuleFor(r => r.Description)
            .NotEmpty()
            .WithMessage(string.Format(ValidationMessages.IsRequired, nameof(UpdateIssueRequest.Description)));
    }
}