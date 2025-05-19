using FastEndpoints;
using FluentValidation;
using Shared.Contracts.Requests.Issues;

namespace API.Validation.Issues;

public sealed class CreateIssueRequestValidator : Validator<CreateIssueRequest>
{
    public CreateIssueRequestValidator()
    {
        RuleFor(r => r.Title)
            .NotEmpty()
            .WithMessage(string.Format(ValidationMessages.IsRequired, nameof(CreateIssueRequest.Title)));

        RuleFor(r => r.Description)
            .NotEmpty()
            .WithMessage(string.Format(ValidationMessages.IsRequired, nameof(CreateIssueRequest.Description)));

        RuleFor(r => r.Client)
            .IsInEnum()
            .WithMessage(string.Format(ValidationMessages.IsRequired, nameof(CreateIssueRequest.Client)));

        RuleFor(r => r.RepositoryOwner)
            .NotEmpty()
            .WithMessage(string.Format(ValidationMessages.IsRequired, nameof(CreateIssueRequest.RepositoryOwner)));

        RuleFor(r => r.RepositoryName)
            .NotEmpty()
            .WithMessage(string.Format(ValidationMessages.IsRequired, nameof(CreateIssueRequest.RepositoryName)));
    }
}
