using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _cache;

        public ExternalUserService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            string cacheKey = $"user_{userId}";

            if (_cache.TryGetValue(cacheKey, out User cachedUser))
            {
                return cachedUser;
            }

            var response = await _httpClient.GetAsync($"users/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"User not found. Status Code: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();

            var json = JsonSerializer.Deserialize<JsonElement>(content);
            var userJson = json.GetProperty("data").GetRawText();

            var dto = JsonSerializer.Deserialize<UserDto>(userJson);

            var user = new User
            {
                Id = dto.id,
                Email = dto.email,
                FirstName = dto.first_name,
                LastName = dto.last_name
            };
            _cache.Set(cacheKey, user, TimeSpan.FromMinutes(5));
            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            const string cacheKey = "all_users";

            if (_cache.TryGetValue(cacheKey, out List<User> cachedUsers))
            {
                return cachedUsers;
            }

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
            _cache.Set(cacheKey, users, TimeSpan.FromMinutes(5));

            return users;
        }
    }
}
