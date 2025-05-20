using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Services.Abstractions;
using Shared.Contracts.Requests.Issues;

namespace API.Endpoints;

[HttpPut("issues/close"), AllowAnonymous]
public sealed class CloseIssueEndpoint(IGitClientFactory gitClientFactory) : Endpoint<CloseIssueRequest>
{
    public override async Task HandleAsync(CloseIssueRequest request, CancellationToken cancellationToken)
    {
        var gitClient = gitClientFactory.GetGitClient(request.Client);
        await gitClient.CloseIssueAsync(request, cancellationToken);

        await SendOkAsync(cancellationToken);
    }
}