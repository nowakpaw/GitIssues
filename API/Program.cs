using FastEndpoints;
using FastEndpoints.Swagger;
using Services;
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

        builder.Services.AddHttpClient();
        builder.Services.AddGitClients(builder.Configuration);

        builder.Services.AddScoped<GitLabService>();
        builder.Services.AddScoped<GitHubService>();
        builder.Services.AddScoped<GitServiceFactory>();

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