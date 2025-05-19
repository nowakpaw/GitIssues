using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Services.Abstractions;
using Shared.Contracts.Requests.Issues;
using Shared.Contracts.Responses.Issues;

namespace API.Endpoints;

[HttpPost("issues/create"), AllowAnonymous]
public sealed class CreateIssueEndpoint(IGitClientFactory gitServiceFactory) : Endpoint<CreateIssueRequest, CreateOrUpdateIssueResponse>
{
    public override async Task HandleAsync(CreateIssueRequest request, CancellationToken cancellationToken)
    {
        var gitService = gitServiceFactory.GetService(request.GitServiceType);
        var responseUri = await gitService.CreateIssueAsync(request, cancellationToken);
        var response = new CreateOrUpdateIssueResponse(responseUri);

        await SendOkAsync(response, cancellationToken);
    }
}