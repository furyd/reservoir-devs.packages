using FluentAssertions;
using ReservoirDevs.Correlation.Models;
using Xunit;

namespace ReservoirDevs.Correlation.Tests.Unit
{
    public class CorrelationHeaderTests
    {
        [Fact]
        public void CorrelationHeader_MapsToString()
        {
            var value = new CorrelationHeader("test");
            var sut = (string)value;

            sut.Should().NotBeNullOrWhiteSpace();
            sut.Should().Be(value.ToString());
        }

        [Fact]
        public void String_MapsToCorrelationHeader()
        {
            const string value = "test";
            var sut = (CorrelationHeader)value;

            sut.Should().NotBeNull();
            sut.ToString().Should().Be(value);
        }
    }
}