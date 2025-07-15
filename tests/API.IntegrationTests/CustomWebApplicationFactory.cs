using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence.Postgres.Context;
using System.Linq;
using System.IO;
using System;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Xunit;
using Microsoft.AspNetCore.TestHost;

namespace API.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;

    public CustomWebApplicationFactory()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpassword")
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseTestServer(options =>
        {
            options.AllowSynchronousIO = true;
        });
        
        var solutionDir = FindSolutionDirectory();
        builder.UseContentRoot(Path.Combine(solutionDir, "src", "Presentation", "API"));
        builder.ConfigureAppConfiguration((context, conf) =>
        {
            var testConfigPath = Path.Combine(solutionDir, "tests", "API.IntegrationTests", "appsettings.Testing.json");
            conf.AddJsonFile(testConfigPath, optional: false);
        });
        
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });

            using var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate();
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
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