using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Services;
using Shared.Contracts.Requests.Issues;
using Shared.Contracts.Responses.Issues;

namespace API.Endpoints;

[HttpPost("issues/create"), AllowAnonymous]
public sealed class CreateIssueEndpoint(GitServiceFactory gitServiceFactory) : Endpoint<CreateIssueRequest, CreateIssueResponse>
{
    private readonly GitServiceFactory _gitServiceFactory = gitServiceFactory;

    public override async Task HandleAsync(CreateIssueRequest request, CancellationToken cancellationToken)
    {
        var gitService = _gitServiceFactory.GetService(request.GitServiceType);
        var issueId = await gitService.CreateIssueAsync(request.Title, request.Description, cancellationToken);

        var response = new CreateIssueResponse(issueId);
        await SendOkAsync(response, cancellationToken);
    }
}
