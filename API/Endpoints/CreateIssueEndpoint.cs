using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Services;
using Shared.Contracts.Requests.Issues;
using Shared.Contracts.Responses.Issues;

namespace API.Endpoints;

[HttpPost("issues/create"), AllowAnonymous]
public sealed class CreateIssueEndpoint(GitClientFactory gitServiceFactory) : Endpoint<CreateIssueRequest, CreateOrUpdateIssueResponse>
{
    private readonly GitClientFactory _gitServiceFactory = gitServiceFactory;

    public override async Task HandleAsync(CreateIssueRequest request, CancellationToken cancellationToken)
    {
        var gitService = _gitServiceFactory.GetService(request.GitServiceType);
        var responseUri = await gitService.CreateIssueAsync(request, cancellationToken);
        var response = new CreateOrUpdateIssueResponse(responseUri);

        await SendOkAsync(response, cancellationToken);
    }
}