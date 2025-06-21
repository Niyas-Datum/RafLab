using Microsoft.Extensions.Options;
using Moq.Protected;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RafLab.Application.Services;
using RafLab.Shared.Dto;
using RafLab.Core.Infrastucture;
using Microsoft.EntityFrameworkCore;

namespace RafLab.UserTest
{
    public class ExternalUserServiceTest
    {

        private ExternalUserService CreateService(string responseJson, string requestUrl, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains(requestUrl)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                });
            // Update the code where the error occurs
            var dbOptions = new DbContextOptionsBuilder<ReqresUserDbContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString()) // ensures isolation per test
               .Options;

            var dbContext = new ReqresUserDbContext(dbOptions);
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://reqres.in/api/")
            };
            httpClient.DefaultRequestHeaders.Add("x-api-key", "reqres-free-v1");

            var options = Options.Create(new ApiSettings
            {
                BaseUrl = "https://reqres.in/api/",
                ApiKey = "reqres-free-v1"
            });

            return new ExternalUserService(httpClient, options, dbContext);
        }

        // Case:  Get User by ID -  
        // Response: 200( Valid )

        [Fact]
        public async Task GetUserById_ValidResponse()
        {
            var json = @"{
          ""data"": {
            ""id"": 10,
            ""email"": ""byron.fields@reqres.in"",
            ""first_name"": ""Byron"",
            ""last_name"": ""Fields"",
            ""avatar"": ""https://reqres.in/img/faces/10-image.jpg""
          }
        }";
          

            var service = CreateService(json, "users/2");

            var result = await service.GetUserByIdAsync(2);

           Assert.NotNull(result);
            Assert.Equal(10, result!.Id);
            Assert.Equal("Byron Fields", result.FullName);
        }
        /// <summary>
        /// Get All users
        /// Returns All Users Across Pages
        /// </summary>
        /// <returns></returns>

        [Fact]
        public async Task GetAllUserAsync()
        {
            var page1 = @"{
            ""page"": 1,
            ""per_page"": 2,
            ""total"": 4,
            ""total_pages"": 2,
            ""data"": [
                { ""id"": 1, ""email"": ""a@a.com"", ""first_name"": ""A"", ""last_name"": ""One"", ""avatar"": ""x"" },
                { ""id"": 2, ""email"": ""b@b.com"", ""first_name"": ""B"", ""last_name"": ""Two"", ""avatar"": ""y"" }
            ]
        }";
            var page2 = @"{
            ""page"": 2,
            ""per_page"": 2,
            ""total"": 4,
            ""total_pages"": 2,
            ""data"": [
                { ""id"": 3, ""email"": ""c@c.com"", ""first_name"": ""C"", ""last_name"": ""Three"", ""avatar"": ""z"" },
                { ""id"": 4, ""email"": ""d@d.com"", ""first_name"": ""D"", ""last_name"": ""Four"", ""avatar"": ""w"" }
            ]
        }";

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(page1, Encoding.UTF8, "application/json")
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(page2, Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://reqres.in/api/")
            };
            httpClient.DefaultRequestHeaders.Add("x-api-key", "reqres-free-v1");
            var dbOptions = new DbContextOptionsBuilder<ReqresUserDbContext>()
           .UseInMemoryDatabase(Guid.NewGuid().ToString()) // ensures isolation per test
           .Options;

            var dbContext = new ReqresUserDbContext(dbOptions);


            var service = new ExternalUserService(httpClient, Options.Create(new ApiSettings
            {
                BaseUrl = "https://reqres.in/api/",
                ApiKey = "reqres-free-v1"
            }), dbContext);

            var users = (await service.GetAllUsersAsync()).ToList();

            Assert.Equal(4, users.Count);
            Assert.Contains(users, u => u.FullName == "C Three");
        }

        /// <summary>
        /// When User Not Found
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUseNotFound()
        {
            var service = CreateService("{}", "users/999", HttpStatusCode.NotFound);
            var result = await service.GetUserByIdAsync(999);
            Assert.Null(result);
        }
        /// <summary>
        /// When Deserialization Fails
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUserDeserializationFails()
        {
            var invalidJson = @"{ ""data"": { ""id"": ""invalid_string"" } }"; // id should be int
            var service = CreateService(invalidJson, "users/2");

            await Assert.ThrowsAsync<ApplicationException>(() => service.GetUserByIdAsync(2));
        }
        /// <summary>
        /// When Server Error
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUsernServerError()
        {
            var service = CreateService("{}", "users/2", HttpStatusCode.InternalServerError);

            await Assert.ThrowsAsync<HttpRequestException>(() => service.GetUserByIdAsync(2));
        }
    }
}
