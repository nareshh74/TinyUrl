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
    }
}
