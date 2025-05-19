namespace Shared.Contracts.Requests.Issues;

public sealed class CloseIssueRequest : BaseIssueRequest
{
    public int Id { get; init; }
}
