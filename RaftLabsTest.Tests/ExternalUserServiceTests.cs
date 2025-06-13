using Moq;
using RaftLabsTest.Infrastructure;
using RaftLabsTest.Models;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

using Xunit;

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
            var service = new ExternalUserService(client);

            // Act
            var result = await service.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("George", result.FirstName);
        }
    }
}
