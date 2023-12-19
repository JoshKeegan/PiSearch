using System.Net;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using StringSearch.Api.Extensions;
using Xunit;

namespace UnitTests.Api.Extensions
{
    public class HttpRequestExtensionsTests
    {
        [Theory]
        [InlineData("119.140.0.200")]
        [InlineData("127.0.0.1")]
        [InlineData("192.168.0.1")]
        [InlineData("::1")]
        [InlineData("2001:0db8:85a3:0000:0000:8a2e:0370:7334")]
        public void GetClientIp_ForwardedForSingle_ReturnsForwardedFor(string strIp)
        {
            // Arrange
            HttpRequest request = Substitute.For<HttpRequest>();
            request.Headers.Returns(new HeaderDictionary()
            {
                { "X-Forwarded-For", new Microsoft.Extensions.Primitives.StringValues(strIp) }
            });

            IPAddress expected = IPAddress.Parse(strIp);

            // Act
            IPAddress actual = request.GetClientIp();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("119.140.0.200")]
        [InlineData("127.0.0.1")]
        [InlineData("192.168.0.1")]
        [InlineData("::1")]
        [InlineData("2001:0db8:85a3:0000:0000:8a2e:0370:7334")]
        public void GetClientIp_NoForwardedFor_ReturnsHttpContextConnectionRemoteIpAddress(string strIp)
        {
            // Arrange
            IPAddress expected = IPAddress.Parse(strIp);

            HttpRequest request = Substitute.For<HttpRequest>();
            request.HttpContext.Connection.RemoteIpAddress.Returns(expected);

            // Act
            IPAddress actual = request.GetClientIp();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
