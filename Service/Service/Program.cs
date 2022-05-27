using System.Buffers.Binary;
using System.IO.Compression;

// The primary namespace of the application
namespace Mapster
{
    class Program
    {
        static void Main(string[] args)
        {
            using FileStream fs = new FileStream(@"", FileMode.Open);

            byte[] buffer = new byte[4];
            fs.Read(buffer, 0, buffer.Length);
            int headerSize = BinaryPrimitives.ReadInt32BigEndian(buffer.AsSpan());

            byte[] headerBuffer = new byte[headerSize];
            fs.Read(headerBuffer, 0, headerBuffer.Length);
            BlobHeader blobHeader = BlobHeader.Parser.ParseFrom(headerBuffer);

            byte[] blobBuffer = new byte[blobHeader.Datasize];
            fs.Read(blobBuffer, 0, blobBuffer.Length);
            Blob blob = Blob.Parser.ParseFrom(blobBuffer);

            var compressedData = blob.ZlibData.Span;
            MemoryStream ms = new MemoryStream(compressedData.ToArray());
            ZLibStream zlibStream = new ZLibStream(ms, CompressionMode.Decompress);
            HeaderBlock headerBlock = HeaderBlock.Parser.ParseFrom(zlibStream);
        }
    }
}
