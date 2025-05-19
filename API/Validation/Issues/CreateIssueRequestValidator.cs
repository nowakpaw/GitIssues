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
            .WithMessage("Title is requied"); //todo 2025/05/19 in real world - move it to resx or something like that

        RuleFor(r => r.Description)
            .NotEmpty()
            .WithMessage("Description is requied"); //todo 2025/05/19 in real world - move it to resx or something like that

        RuleFor(r => r.GitServiceType)
            .IsInEnum()
            .WithMessage("GitServiceType is not valid"); //todo 2025/05/19 in real world - move it to resx or something like that
    }
}
