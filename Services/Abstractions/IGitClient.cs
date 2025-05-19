using Shared.Contracts.Requests.Issues;
using Shared.Enums;

namespace Services.Abstractions;

public interface IGitClient
{
    GitClientTypes ClientType { get; }
    Task<string> CreateIssueAsync(CreateIssueRequest request, CancellationToken cancellationToken);
    Task<string> UpdateIssueAsync(UpdateIssueRequest request, CancellationToken cancellationToken);
    Task CloseIssueAsync(CloseIssueRequest request, CancellationToken cancellationToken);
}