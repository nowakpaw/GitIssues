using Services.Abstractions;
using Shared.Enums;

namespace Services;

public class GitClientFactory(IEnumerable<IGitClient> clients) : IGitClientFactory
{
    public IGitClient GetService(GitClients client)
    {
        var result = clients.FirstOrDefault(c => c.Client == client);
        if (result is not null)
        {
            return result;
        }

        throw new NotSupportedException($"Client {client} is not supported.");
    }
}