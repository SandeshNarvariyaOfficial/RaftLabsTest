using RaftLabsTest.Application;
using RaftLabsTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RaftLabsTest.Infrastructure
{
    public class ExternalUserService : IExternalUserService
    {
        private readonly HttpClient _httpClient;

        public ExternalUserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"users/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"User not found. Status Code: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();

            var json = JsonSerializer.Deserialize<JsonElement>(content);
            var userJson = json.GetProperty("data").GetRawText();

            var dto = JsonSerializer.Deserialize<UserDto>(userJson);

            return new User
            {
                Id = dto.id,
                Email = dto.email,
                FirstName = dto.first_name,
                LastName = dto.last_name
            };
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            int page = 1;

            while (true)
            {
                var response = await _httpClient.GetAsync($"users?page={page}");

                if (!response.IsSuccessStatusCode)
                    break;

                var content = await response.Content.ReadAsStringAsync();
                var pageData = JsonSerializer.Deserialize<UserListResponse>(content);

                if (pageData?.data == null || !pageData.data.Any())
                    break;

                users.AddRange(pageData.data.Select(dto => new User
                {
                    Id = dto.id,
                    Email = dto.email,
                    FirstName = dto.first_name,
                    LastName = dto.last_name
                }));
                page++;
            }
            return users;
        }
    }
}
