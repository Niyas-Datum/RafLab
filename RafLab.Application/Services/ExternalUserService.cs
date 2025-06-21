using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RafLab.Application.Dto;
using RafLab.Application.Mapper;
using RafLab.Application.Services._i;
using RafLab.Core.Infrastucture;
using RafLab.Core.Infrastucture.Models;
using RafLab.Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RafLab.Application.Services
{
    public class ExternalUserService : IExternalUserService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ReqresUserDbContext _dbContext;



        public ExternalUserService(HttpClient httpClient, IOptions<ApiSettings> settings, ReqresUserDbContext dbContext)
        {
            _httpClient = httpClient;    
           // _httpClient.BaseAddress = new Uri(settings.Value.BaseUrl);
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
           // _httpClient.DefaultRequestHeaders.Add("x-api-key", settings.Value.ApiKey);
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));   

        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var allUsers = new List<User>();
            int currentPage = 1;
            int totalPages;

            do
            {
                var response = await _httpClient.GetAsync($"users?page={currentPage}");
                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Error fetching users. Page: {currentPage}");

                using var stream = await response.Content.ReadAsStreamAsync();
                var pageData = await JsonSerializer.DeserializeAsync<PagedUser>(stream, _jsonOptions);
                if (pageData != null)
                {
                    allUsers.AddRange(pageData.Data);
                    totalPages = pageData.Total_Pages;
                    currentPage++;
                }
                else
                {
                    throw new ApplicationException("Failed to deserialize paged user data.");
                }

            } while (currentPage <= totalPages);

            return allUsers.ToDtoList();
        }

        public async Task<IEnumerable<UserDto>> GetCacheUser()
        {
            var users = await _dbContext.Users.ToListAsync();

            return users.ToDtoList();
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            try
            {

                var cached = await _dbContext.Users.FindAsync(userId);
                if (cached != null)
                {
                    return new UserDto
                    {
                        Id = cached.Id,
                        FullName = $"{cached.FirstName} {cached.LastName}",
                        Email = cached.Email,
                        Avatar = cached.Avatar
                    };
                }

                var response = await _httpClient.GetAsync($"users/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    using var stream = await response.Content.ReadAsStreamAsync();
                    var json = await JsonDocument.ParseAsync(stream);
                    var user = json.RootElement.GetProperty("data").Deserialize<User>(_jsonOptions);
                    var entity = new User
                    {
                        Id = user.Id,
                        FirstName = user.FirstName ,
                        LastName=  user.LastName,
                        Email = user.Email,
                        Avatar = user.Avatar
                    };

                    _dbContext.Users.Add(entity);
                    await _dbContext.SaveChangesAsync();
                    return user.ToDto();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                throw new HttpRequestException($"Failed to fetch user. Status: {response.StatusCode}");
            }
            catch (JsonException)
            {
                throw new ApplicationException("Failed to deserialize user response.");
            }
        }
    }
}
