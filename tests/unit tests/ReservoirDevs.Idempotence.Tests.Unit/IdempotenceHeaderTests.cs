using FluentAssertions;
using ReservoirDevs.Idempotence.Models;
using Xunit;

namespace ReservoirDevs.Idempotence.Tests.Unit
{
    public class IdempotenceHeaderTests
    {
        [Fact]
        public void String_MapsToIdempotenceHeader()
        {
            const string value = "test";
            var sut = (IdempotenceHeader)value;

            sut.Should().NotBeNull();
            sut.ToString().Should().Be(value);
        }

        [Fact]
        public void IdempotenceHeader_MapsToString()
        {
            var value = new IdempotenceHeader("test");
            var sut = (string)value;

            sut.Should().NotBeNullOrWhiteSpace();
            sut.Should().Be(value.ToString());
        }
    }
}