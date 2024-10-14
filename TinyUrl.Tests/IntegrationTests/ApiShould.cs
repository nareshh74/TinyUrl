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

        [Fact]
        public async Task RedirectShortUrl()
        {
            var client = this._factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false  // Disable automatic redirection following
            });
            var content = new StringContent("{\"longUrl\":\"https://www.google.com\"}", Encoding.UTF8, "application/json");
            var shortenResponse = await client.PostAsync("api/v1/shorten", content);
            var shortUrlResponse = await shortenResponse.Content.ReadFromJsonAsync<ShortUrlResponse>();
            var response = await client.GetAsync($"api/v1/{shortUrlResponse?.ShortUrl}");
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("https://www.google.com/", response.Headers?.Location?.ToString());
        }
    }
}