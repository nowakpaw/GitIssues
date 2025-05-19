using FastEndpoints;
using FluentValidation;
using Shared.Contracts.Requests.Issues;

namespace API.Validation.Issues;

public abstract class BaseIssueRequestValidator<T> : Validator<T> where T : BaseIssueRequest
{
    protected BaseIssueRequestValidator()
    {
        RuleFor(r => r.Client)
            .IsInEnum()
            .WithMessage(string.Format(ValidationMessages.NotSupported, nameof(BaseIssueRequest.Client)));

        RuleFor(r => r.RepositoryOwner)
            .NotEmpty()
            .WithMessage(string.Format(ValidationMessages.IsRequired, nameof(BaseIssueRequest.RepositoryOwner)));

        RuleFor(r => r.RepositoryName)
            .NotEmpty()
            .WithMessage(string.Format(ValidationMessages.IsRequired, nameof(BaseIssueRequest.RepositoryName)));
    }
}