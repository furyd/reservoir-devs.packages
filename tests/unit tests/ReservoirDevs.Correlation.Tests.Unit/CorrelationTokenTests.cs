using FluentAssertions;
using ReservoirDevs.Correlation.Models;
using Xunit;

namespace ReservoirDevs.Correlation.Tests.Unit
{
    public class CorrelationTokenTests
    {
        [Fact]
        public void String_MapsToCorrelationToken()
        {
            const string value = "test";
            var sut = (CorrelationToken) value;

            sut.Should().NotBeNull();
            sut.ToString().Should().Be(value);
        }

        [Fact]
        public void CorrelationToken_MapsToString()
        {
            var value = new CorrelationToken("test");
            var sut = (string) value;

            sut.Should().NotBeNullOrWhiteSpace();
            sut.Should().Be(value.ToString());
        }
    }
}