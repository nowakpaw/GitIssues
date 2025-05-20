using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Services.Abstractions;
using Shared.Contracts.Requests.Issues;
using Shared.Contracts.Responses.Issues;

namespace API.Endpoints;

[HttpPost("issues/create"), AllowAnonymous]
public sealed class CreateIssueEndpoint(IGitClientFactory gitClientFactory) : Endpoint<CreateIssueRequest, CreateOrUpdateIssueResponse>
{
    public override async Task HandleAsync(CreateIssueRequest request, CancellationToken cancellationToken)
    {
        var gitClient = gitClientFactory.GetGitClient(request.Client);
        var responseUri = await gitClient.CreateIssueAsync(request, cancellationToken);
        var response = new CreateOrUpdateIssueResponse(responseUri);

        await SendOkAsync(response, cancellationToken);
    }
}