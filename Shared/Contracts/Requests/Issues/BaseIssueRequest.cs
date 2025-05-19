using Shared.Enums;

namespace Shared.Contracts.Requests.Issues;

public abstract class BaseIssueRequest
{
    public string RepositoryName { get; init; } = null!;
    public string RepositoryOwner { get; init; } = null!;
    public GitClientTypes GitServiceType { get; init; }
}