using System.IO;
using System.Threading.Tasks;
using Microsoft.IO;

namespace ReservoirDevs.Logging.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<string> ReadInChunks(this Stream stream, int chunkSize)
        {
            if (stream.Length == 0 || chunkSize < 1)
            {
                return string.Empty;
            }
            
            var recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();

            await using (var localStream = recyclableMemoryStreamManager.GetStream())
            {
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(localStream);
                stream.Seek(0, SeekOrigin.Begin);
                localStream.Seek(0, SeekOrigin.Begin);

                await using (var textWriter = new StringWriter())
                {
                    using (var reader = new StreamReader(localStream))
                    {
                        var readChunk = new char[chunkSize];
                        int readChunkLength;

                        do
                        {
                            readChunkLength = await reader.ReadBlockAsync(readChunk, 0, chunkSize).ConfigureAwait(false);
                            await textWriter.WriteAsync(readChunk, 0, readChunkLength);
                        } while (readChunkLength > 0);

                        return textWriter.ToString();
                    }
                }
            }
        }
    }
}