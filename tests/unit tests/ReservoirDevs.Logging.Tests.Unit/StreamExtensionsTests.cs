using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using ReservoirDevs.Logging.Extensions;
using Xunit;

namespace ReservoirDevs.Logging.Tests.Unit
{
    public class StreamExtensionsTests
    {
        [Fact]
        public async Task ReadInChunks_ReturnsEmptyString_WhenStreamHasNoContent()
        {
            var sut = new MemoryStream();

            var result = await sut.ReadInChunks(1024);

            result.Should().BeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ReadInChunks_ReturnsEmptyString_WhenStreamIsEmpty()
        {
            var sut = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(""));

            var result = await sut.ReadInChunks(1024);

            result.Should().BeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task ReadInChunks_ReturnsEmptyString_WhenChunkSizeIsLessThanOne(int value)
        {
            var sut = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("test"));

            var result = await sut.ReadInChunks(value);

            result.Should().BeNullOrWhiteSpace();
        }
    }
}