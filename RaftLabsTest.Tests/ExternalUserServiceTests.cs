using Moq;
using RaftLabsTest.Infrastructure;
using RaftLabsTest.Models;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

using Xunit;
using Microsoft.Extensions.Caching.Memory;

namespace RaftLabsTest.Tests
{
    public class ExternalUserServiceTests
    {
        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser()
        {
            // Arrange
            var expectedUser = new User
            {
                Id = 1,
                Email = "george.bluth@reqres.in",
                FirstName = "George",
                LastName = "Bluth",
                Avatar = "https://reqres.in/img/faces/1-image.jpg"
            };

            var response = new
            {
                data = expectedUser
            };

            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(response)
            };

            var handler = new MockHttpMessageHandler();
            handler.When("https://reqres.in/api/users/1")
                   .Respond(_ => message);

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://reqres.in/api/")
            };

            var loggerMock = new Mock<ILogger<ExternalUserService>>();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var service = new ExternalUserService(client, memoryCache);

            // Act
            var result = await service.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("George", result.FirstName);
        }
        [Fact]
        public async Task GetAllUsersAsync_ReturnsListOfUsers()
        {
            var expectedUsers = new[]
            {
                new UserDto
                {
                    id = 1,
                    email = "george.bluth@reqres.in",
                    first_name = "George",
                    last_name = "Bluth",
                    avatar = "https://reqres.in/img/faces/1-image.jpg"
                },
                new UserDto
                {
                    id = 2,
                    email = "janet.weaver@reqres.in",
                    first_name = "Janet",
                    last_name = "Weaver",
                    avatar = "https://reqres.in/img/faces/2-image.jpg"
                }
            };

            var pageResponse = new
            {
                page = 1,
                per_page = 6,
                total = 12,
                total_pages = 2,
                data = expectedUsers
            };

            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(pageResponse)
            };

            var handler = new MockHttpMessageHandler();
            handler.When("https://reqres.in/api/users?page=1")
                   .Respond(_ => message);
            handler.When("https://reqres.in/api/users?page=2") // simulate end of data
                   .Respond(_ => new HttpResponseMessage(HttpStatusCode.OK)
                   {
                       Content = JsonContent.Create(new { data = Array.Empty<UserDto>() })
                   });

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://reqres.in/api/")
            };

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var service = new ExternalUserService(client, memoryCache);

            // Act
            var result = (await service.GetAllUsersAsync()).ToList();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("George", result[0].FirstName);
            Assert.Equal("Janet", result[1].FirstName);
        }

    }
}
