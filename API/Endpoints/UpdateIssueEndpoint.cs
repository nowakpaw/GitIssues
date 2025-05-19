using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Services;
using Shared.Contracts.Requests.Issues;
using Shared.Contracts.Responses.Issues;

namespace API.Endpoints;

[HttpPut("issues/update"), AllowAnonymous]
public sealed class UpdateIssueEndpoint(GitClientFactory gitServiceFactory) : Endpoint<UpdateIssueRequest, CreateOrUpdateIssueResponse>
{
    private readonly GitClientFactory _gitServiceFactory = gitServiceFactory;

    public override async Task HandleAsync(UpdateIssueRequest request, CancellationToken cancellationToken)
    {
        var gitService = _gitServiceFactory.GetService(request.GitServiceType);
        var responseUri = await gitService.UpdateIssueAsync(request, cancellationToken);
        var response = new CreateOrUpdateIssueResponse(responseUri);

        await SendOkAsync(response, cancellationToken);
    }
}