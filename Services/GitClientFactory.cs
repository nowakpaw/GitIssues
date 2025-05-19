using Services.Abstractions;
using Shared.Enums;

namespace Services;

public class GitClientFactory(IEnumerable<IGitClient> clients) : IGitClientFactory
{
    private readonly Dictionary<GitClientTypes, IGitClient> _clients = clients.ToDictionary(c => c.ClientType);

    public IGitClient GetService(GitClientTypes type)
    {
        if (_clients.TryGetValue(type, out var client))
        {
            return client;
        }

        throw new NotSupportedException($"Client type {type} is not supported.");
    }
}