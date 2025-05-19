using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Services.Abstractions;
using Shared.Contracts.Requests.Issues;

namespace API.Endpoints;

[HttpPut("issues/close"), AllowAnonymous]
public sealed class CloseIssueEndpoint(IGitClientFactory gitServiceFactory) : Endpoint<CloseIssueRequest>
{
    public override async Task HandleAsync(CloseIssueRequest request, CancellationToken cancellationToken)
    {
        var gitService = gitServiceFactory.GetService(request.GitServiceType);
        await gitService.CloseIssueAsync(request, cancellationToken);

        await SendOkAsync(cancellationToken);
    }
}