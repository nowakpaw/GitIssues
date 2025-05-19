using Shared.Contracts.Requests.Issues;

namespace Services.Abstractions;

public interface IGitService
{
    Task<string> CreateIssueAsync(CreateIssueRequest request, CancellationToken cancellationToken);
    Task<string> UpdateIssueAsync(UpdateIssueRequest request, CancellationToken cancellationToken);
    Task CloseIssueAsync(CloseIssueRequest request, CancellationToken cancellationToken);
}