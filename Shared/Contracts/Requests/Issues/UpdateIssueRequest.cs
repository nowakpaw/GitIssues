namespace Shared.Contracts.Requests.Issues;

public sealed class UpdateIssueRequest : CreateIssueRequest
{
    public int Id { get; init; }
}
