using Shared.Enums;

namespace Services.Abstractions;

public interface IGitClientFactory
{
    IGitClient GetGitClient(GitClients type);
}