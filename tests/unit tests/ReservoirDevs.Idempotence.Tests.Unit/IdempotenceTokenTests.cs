using FluentAssertions;
using ReservoirDevs.Idempotence.Models;
using Xunit;

namespace ReservoirDevs.Idempotence.Tests.Unit
{
    public class IdempotenceTokenTests
    {
        [Fact]
        public void IdempotenceToken_MapsToString()
        {
            var value = new IdempotenceToken("test");
            var sut = (string)value;

            sut.Should().NotBeNullOrWhiteSpace();
            sut.Should().Be(value.ToString());
        }

        [Fact]
        public void String_MapsToIdempotenceToken()
        {
            const string value = "test";
            var sut = (IdempotenceToken)value;

            sut.Should().NotBeNull();
            sut.ToString().Should().Be(value); 
        }
    }
}