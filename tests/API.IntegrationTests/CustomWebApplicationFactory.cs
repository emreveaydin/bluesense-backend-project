using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Infrastructure.Persistence.Postgres.Context;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;

namespace API.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var solutionDir = FindSolutionDirectory();
        var projectDir = Path.Combine(solutionDir, "src", "Presentation", "API");

        builder.UseContentRoot(projectDir);
        builder.ConfigureAppConfiguration((context, conf) =>
        {
            var testConfigPath = Path.Combine(solutionDir, "tests", "API.IntegrationTests", "appsettings.Testing.json");
            conf.AddJsonFile(testConfigPath);
        });

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            var dbConnectionDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(ApplicationDbContext));
            if (dbConnectionDescriptor != null)
            {
                services.Remove(dbConnectionDescriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });
        });

        builder.UseEnvironment("Testing");
    }

    private static string FindSolutionDirectory()
    {
        var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);
        while (currentDirectory != null && !currentDirectory.GetFiles("*.sln").Any())
        {
            currentDirectory = currentDirectory.Parent;
        }
        return currentDirectory?.FullName ?? throw new DirectoryNotFoundException("Solution directory not found.");
    }
}