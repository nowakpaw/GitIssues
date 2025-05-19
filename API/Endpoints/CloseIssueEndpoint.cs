using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Services;
using Shared.Contracts.Requests.Issues;

namespace API.Endpoints;

[HttpPut("issues/close"), AllowAnonymous]
public sealed class CloseIssueEndpoint(GitServiceFactory gitServiceFactory) : Endpoint<CloseIssueRequest>
{
    private readonly GitServiceFactory _gitServiceFactory = gitServiceFactory;

    public override async Task HandleAsync(CloseIssueRequest request, CancellationToken cancellationToken)
    {
        var gitService = _gitServiceFactory.GetService(request.GitServiceType);
        await gitService.CloseIssueAsync(request, cancellationToken);

        await SendOkAsync(cancellationToken);
    }
}