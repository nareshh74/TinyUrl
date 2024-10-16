using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using TinyUrl.Controllers.Models;

namespace TinyUrl.Tests
{
    public class ApiShould : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ApiShould(WebApplicationFactory<Program> factory)
        {
            this._factory = factory;
        }

        [Fact]
        public async Task ShortenUrl()
        {
            var client = this._factory.CreateClient();
            var content = new StringContent("{\"longUrl\":\"https://www.google.com\"}", Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/v1/shorten", content);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var shortUrlResponse = await response.Content.ReadFromJsonAsync<ShortUrlResponse>();
            Assert.NotNull(shortUrlResponse);
            Assert.NotEmpty(shortUrlResponse.ShortUrl);
            Assert.True(shortUrlResponse.ShortUrl.Length < 8);
        }

        [Theory]
        [InlineData("https://www.google.com", "https://www.microsoft.com", false)]
        [InlineData("https://www.google.com", "https://www.google.com", true)]
        [InlineData("https://www.google.com", "https://www.google.com/path", false)]
        [InlineData("https://www.google.com", "https://www.google.com?q=1", false)]
        public async Task RedirectShortUrl(string url1String, string url2String, bool equalFlag)
        {
            var client = this._factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false // Disable automatic redirection following
            });
            var content1 = new StringContent("{\"longUrl\":\"" + url1String + "\"}", Encoding.UTF8, "application/json");
            var task1 = client.PostAsync("api/v1/shorten", content1);
            var content2 = new StringContent("{\"longUrl\":\"" + url2String + "\"}", Encoding.UTF8, "application/json");
            var task2 = client.PostAsync("api/v1/shorten", content2);
            var results = await Task.WhenAll(task1, task2);
            var shortUrlResponse1 = await results[0].Content.ReadFromJsonAsync<ShortUrlResponse>();
            var shortUrlResponse2 = await results[1].Content.ReadFromJsonAsync<ShortUrlResponse>();
            var response1 = await client.GetAsync($"api/v1/{shortUrlResponse1?.ShortUrl}");
            var response2 = await client.GetAsync($"api/v1/{shortUrlResponse2?.ShortUrl}");
            Assert.Equal(HttpStatusCode.Redirect, response1.StatusCode);
            Assert.Equal(HttpStatusCode.Redirect, response2.StatusCode);
            Assert.Equal(url1String == response1.Headers.Location?.ToString(), equalFlag);
            Assert.Equal(url2String == response2.Headers.Location?.ToString(), equalFlag);
        }
    }
}