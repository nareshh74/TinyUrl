using System.Collections.Concurrent;
using TinyUrl.Logic;

namespace TinyUrl.Tests.UnitTests
{
    public class UrlConverterShould
    {
        [Fact]
        public void ConvertGivenUrlToAStringWithLengthLessThan8()
        {
            var urlConverter = new UrlConverter();
            var url = new Uri("https://www.google.com");
            var code = urlConverter.Encode(url);
            Assert.True(code.Length <= 8);
        }

        [Fact]
        public void ConvertGivenCodeBackToCorrectUrl()
        {
            var urlConverter = new UrlConverter();
            var url = new Uri("https://www.google.com");
            var decodedUrl = urlConverter.Decode(urlConverter.Encode(url));
            Assert.NotNull(decodedUrl);
            Assert.Equal(url, decodedUrl);
        }

        [Theory]
        [InlineData("https://www.google.com", "https://www.microsoft.com", false)]
        [InlineData("https://www.google.com", "https://www.google.com", true)]
        [InlineData("https://www.google.com", "https://www.google.com/path", false)]
        [InlineData("https://www.google.com", "https://www.google.com?q=1", false)]
        public void EncodeAndDecodeAsExpected(string url1String, string url2String, bool equalFlag)
        {
            var urlConverter = new UrlConverter();
            var url1 = new Uri(url1String);
            var url2 = new Uri(url2String);
            var urlToCode = new ConcurrentDictionary<Uri, string>();
            urlToCode[url1] = "";
            urlToCode[url2] = "";
            Parallel.ForEach(urlToCode.Keys, url => urlToCode[url] = urlConverter.Encode(url));
            Assert.Equal(equalFlag, urlToCode.Values.First() == urlToCode.Values.Last());
            Parallel.ForEach(urlToCode.Keys, url => Assert.Equal(url, urlConverter.Decode(urlToCode[url])));
        }
    }
}
