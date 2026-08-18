using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;
using TaskManager.Infrastructure.Data;
using Xunit;

namespace TaskManager.Api.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("TaskManager_Test")
            .WithUsername("postgres")
            .WithPassword("749382")
            .Build();

        public async Task InitializeAsync()
        {
            Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "MY_SUPER_SECRET_KEY_12345_DEKU_MIDORIA");
            await _dbContainer.StartAsync();
            Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", _dbContainer.GetConnectionString());
        }

        public new async Task DisposeAsync()
        {
            await _dbContainer.StopAsync();
            Environment.SetEnvironmentVariable("JWT_SECRET_KEY", null);
            Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", null);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "ConnectionStrings:DefaultConnection", _dbContainer.GetConnectionString() },
                    { "JWT_SECRET_KEY", "MY_SUPER_SECRET_KEY_12345_DEKU_MIDORIA" }
                });
            });

            builder.ConfigureTestServices(services =>
            {
                // Remove the HostedService during testing so background worker doesn't run concurrently
                var backgroundWorker = services.SingleOrDefault(
                    d => d.ServiceType == typeof(Microsoft.Extensions.Hosting.IHostedService) && 
                         d.ImplementationType?.Name == "TaskMaintenanceWorker");
                if (backgroundWorker != null)
                {
                    services.Remove(backgroundWorker);
                }

                // Add fake authentication handler
                services.AddAuthentication(defaultScheme: "TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
            });
        }

        // Helper to retrieve DB context in tests
        public AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(_dbContainer.GetConnectionString())
                .Options;
            
            var context = new AppDbContext(options);
            return context;
        }
    }

    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, "1"), // Mock UserId = 1
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.Email, "testuser@example.com"),
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}
