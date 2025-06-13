using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using RaftLabsTest.Infrastructure;
using RaftLabsTest.Models;
using RichardSzalay.MockHttp;
using System.Text.Json;

namespace RaftLabsTest.Test
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
        public async Task GetAllUsersAsync_ReturnsUsersAcrossMultiplePages()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();

            // Page 1
            var page1 = new UserListResponse
            {
                data = new List<UserDto>
        {
            new UserDto { id = 1, email = "a@test.com", first_name = "A", last_name = "One", avatar = "a1" },
            new UserDto { id = 2, email = "b@test.com", first_name = "B", last_name = "Two", avatar = "b2" }
        }
            };

            // Page 2
            var page2 = new UserListResponse
            {
                data = new List<UserDto>
        {
            new UserDto { id = 3, email = "c@test.com", first_name = "C", last_name = "Three", avatar = "c3" }
        }
            };

            // Page 3 - empty to end loop
            var page3 = new UserListResponse
            {
                data = new List<UserDto>()
            };

            mockHttp.When("https://reqres.in/api/users?page=1")
                .Respond("application/json", JsonSerializer.Serialize(page1));
            mockHttp.When("https://reqres.in/api/users?page=2")
                .Respond("application/json", JsonSerializer.Serialize(page2));
            mockHttp.When("https://reqres.in/api/users?page=3")
                .Respond("application/json", JsonSerializer.Serialize(page3));

            var client = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("https://reqres.in/api/")
            };
            var cache = new MemoryCache(new MemoryCacheOptions());

            var service = new ExternalUserService(client, cache);

            // Act
            var users = await service.GetAllUsersAsync();

            // Assert
            Assert.NotNull(users);
            Assert.Equal(3, users.Count());

            Assert.Equal("A", users.ElementAt(0).FirstName);
            Assert.Equal("B", users.ElementAt(1).FirstName);
            Assert.Equal("C", users.ElementAt(2).FirstName);
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