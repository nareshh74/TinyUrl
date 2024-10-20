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
            Assert.True(shortUrlResponse.ShortUrl.Length <= 8);
        }

        [Theory]
        [InlineData("https://www.google.com/", "https://www.microsoft.com/", false)]
        [InlineData("https://www.google.com/", "https://www.google.com/", true)]
        [InlineData("https://www.google.com/", "https://www.google.com/path", false)]
        [InlineData("https://www.google.com/", "https://www.google.com/?q=1", false)]
        public async Task RedirectShortUrl(string url1String, string url2String, bool equalFlag, CancellationToken cancellationToken = default)
        {
            var client = this._factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false // Disable automatic redirection following
            });

            var shortenTasks = new List<Task<ShortUrlResponse?>>
            {
                this.CreateShortUrl(client, url1String, cancellationToken),
                this.CreateShortUrl(client, url2String, cancellationToken)
            };

            var shortUrlResponses = await Task.WhenAll<ShortUrlResponse?>(shortenTasks);

            var redirectTasks = shortUrlResponses.Select(shortUrlResponse => client.GetAsync($"api/v1/{shortUrlResponse?.ShortUrl}", cancellationToken)).ToList();

            var redirectResponses = await Task.WhenAll<HttpResponseMessage>(redirectTasks);

            Assert.True(redirectResponses.All(response => response.StatusCode == HttpStatusCode.Redirect));

            Assert.Equal(url1String, redirectResponses[0].Headers.Location?.ToString());
            Assert.Equal(url2String, redirectResponses[1].Headers.Location?.ToString());

            Assert.Equal(url1String == redirectResponses[1].Headers.Location?.ToString(), equalFlag);
            Assert.Equal(url2String == redirectResponses[0].Headers.Location?.ToString(), equalFlag);
        }

        private async Task<ShortUrlResponse?> CreateShortUrl(HttpClient client, string longUrl, CancellationToken cancellationToken)
        {
            var content = JsonContent.Create(new { longUrl });
            var response = await client.PostAsync("api/v1/shorten", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ShortUrlResponse>(cancellationToken);
        }


    }
}