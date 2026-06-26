using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Product.Application.DTOs;
using Product.Infrastructure.Data;
using Xunit;

namespace Product.Api.Tests
{
    public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public AuthIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDatabase");
                    });
                });
            });
        }

        [Fact]
        public async Task GetProducts_WithoutAuthentication_ReturnsUnauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/v1/products");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task RegisterAndLogin_WithValidCredentials_ReturnsTokens()
        {
            var client = _factory.CreateClient();
            var registerRequest = new RegisterRequest
            {
                Username = "testadmin",
                Password = "Password123!",
                Role = "Administrator"
            };

            var registerResponse = await client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);
            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            var registerResult = await registerResponse.Content.ReadFromJsonAsync<TokenResponse>();
            Assert.NotNull(registerResult);
            Assert.NotNull(registerResult.AccessToken);
            Assert.NotNull(registerResult.RefreshToken);
            Assert.Equal("testadmin", registerResult.Username);

            var loginRequest = new LoginRequest
            {
                Username = "testadmin",
                Password = "Password123!"
            };
            var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginResult = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();
            Assert.NotNull(loginResult);
            Assert.NotNull(loginResult.AccessToken);
            Assert.NotNull(loginResult.RefreshToken);
        }
    }
}
