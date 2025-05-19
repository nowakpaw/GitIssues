using FastEndpoints;
using FastEndpoints.Swagger;
using Services;
using Services.Abstractions;
using Shared.Extensions;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddFastEndpoints();
        builder.Services.AddAuthorization(); //todo 2025/05/19 - all endpoints currently have 'allow anonymous' option enabled
        builder.Services.SwaggerDocument();

        builder.Services.AddGitHttpClients(builder.Configuration);

        builder.Services.AddScoped<IGitClient, GitHubClient>();
        builder.Services.AddScoped<IGitClient, GitLabClient>();
        builder.Services.AddScoped<IGitClientFactory, GitClientFactory>();

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseFastEndpoints();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerGen();
        }

        app.Run();
    }
}