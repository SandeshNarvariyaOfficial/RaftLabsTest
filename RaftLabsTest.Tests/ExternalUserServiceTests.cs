﻿
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using RaftLabsTest.Infrastructure;
using RichardSzalay.MockHttp;
using System.Text.Json;
using Xunit;
namespace RaftLabsTest.Tests
{
    public class ExternalUserServiceTests
    {
        private readonly IConfiguration _config;
        public ExternalUserServiceTests()
        {
            var inMemorySettings = new Dictionary<string, string> {
            {"ReqResApi:BaseUrl", "https://reqres.in/api/"},
            {"ReqResApi:ApiKey", "reqres-free-v1"}
        };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsUsersList()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When("https://reqres.in/api/users?page=1")
                .Respond("application/json", JsonSerializer.Serialize(new
                {
                    data = new[]
                    {
                    new { id = 1, email = "george.bluth@reqres.in", first_name = "George", last_name = "Bluth", avatar = "url1" },
                    new { id = 2, email = "janet.weaver@reqres.in", first_name = "Janet", last_name = "Weaver", avatar = "url2" }
                    }
                }));

            var client = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("https://reqres.in/api/")
            };
            client.DefaultRequestHeaders.Add("x-api-key", "dummy-api-key");
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var service = new ExternalUserService(client , memoryCache);

            // Act
            var users = await service.GetAllUsersAsync();

            // Assert
            Assert.NotNull(users);
            Assert.Equal(2, users.Count());
            Assert.Equal("George", users.First().FirstName);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsSingleUser()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When("https://reqres.in/api/users/1")
                .Respond("application/json", JsonSerializer.Serialize(new
                {
                    data = new { id = 1, email = "george.bluth@reqres.in", first_name = "George", last_name = "Bluth", avatar = "url1" }
                }));

            var client = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("https://reqres.in/api/")
            };
            client.DefaultRequestHeaders.Add("x-api-key", "dummy-api-key");
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var service = new ExternalUserService(client, memoryCache);

            // Act
            var user = await service.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(1, user.Id);
            Assert.Equal("George", user.FirstName);
        }
    }
}
