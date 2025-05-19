using Microsoft.Extensions.DependencyInjection;
using Services.Abstractions;
using Shared.Enums;

namespace Services;

public class GitServiceFactory(IServiceProvider provider)
{
    public IGitService GetService(GitServiceTypes type)
    {
        return type switch
        {
            GitServiceTypes.GitHub => provider.GetRequiredService<GitHubService>(),
            GitServiceTypes.GitLab => provider.GetRequiredService<GitLabService>(),
            _ => throw new NotSupportedException()
        };
    }
}