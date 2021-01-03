using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReservoirDevs.Controllers.Extensions;
using Xunit;

namespace ReservoirDevs.Controllers.Tests.Unit
{
    public class ControllerBaseExtensionsTests
    {
        [Fact]
        public async Task PassThrough_Maps_HttpResponseMessageToHttpResponse()
        {
            const string content = "testing";
            const string contentType = "text/text";

            var sut = new TestController
            {
                ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext{ Response = { Body = new MemoryStream()}}}
            };


            var responseMessage = new HttpResponseMessage(HttpStatusCode.Accepted)
            {
                Content = new StringContent(content, Encoding.UTF8, contentType)
            };

            responseMessage.Content.Headers.Add("A", "B");

            await sut.PassThrough(responseMessage);

            sut.Response.StatusCode.Should().Be((int)responseMessage.StatusCode);
            sut.Response.Headers.ContainsKey("A").Should().BeTrue();
            sut.Response.ContentType.Should().Contain(contentType);

            sut.Response.Body.Seek(0, SeekOrigin.Begin);
            using (var test = new StreamReader(sut.Response.Body))
            {
                var responseContent = await test.ReadToEndAsync();
                responseContent.Should().Be(content);
            }
        }
    }
}
