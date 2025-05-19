using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Services.Abstractions;
using Shared.Contracts.Requests.Issues;
using Shared.Contracts.Responses.Issues;

namespace API.Endpoints;

[HttpPut("issues/update"), AllowAnonymous]
public sealed class UpdateIssueEndpoint(IGitClientFactory gitServiceFactory) : Endpoint<UpdateIssueRequest, CreateOrUpdateIssueResponse>
{
    public override async Task HandleAsync(UpdateIssueRequest request, CancellationToken cancellationToken)
    {
        var gitService = gitServiceFactory.GetService(request.Client);
        var responseUri = await gitService.UpdateIssueAsync(request, cancellationToken);
        var response = new CreateOrUpdateIssueResponse(responseUri);

        await SendOkAsync(response, cancellationToken);
    }
}