using System.IO;
using System.Web;

namespace SiteMesh.Asp
{
	public class ResponseFilter : Stream
	{
		private HttpApplication application;
		private Stream originalStream;
		private byte[] buffer;
		private long position;


		public ResponseFilter(HttpApplication app)
		{
			application = app;
			originalStream = app.Response.Filter;
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return true; }
		}

		public override bool CanWrite
		{
			get { return true; }
		}

		public override long Length
		{
			get { return buffer.Length; }
		}

		public override long Position
		{
			get { return position; }
			set { position = value; }
		}


		public override long Seek(long offset, SeekOrigin direction)
		{
			return originalStream.Seek(offset, direction);
		}

		public override void SetLength(long length)
		{
			originalStream.SetLength(length);
		}

		public override void Close()
		{
			originalStream.Close();
		}

		public override void Flush()
		{
			originalStream.Flush();
		}

		public byte[] Buffer
		{
			get { return buffer; }
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return originalStream.Read(buffer, offset, count);
		}

		public override void Write(byte[] buff, int offset, int count)
		{
			if (this.buffer == null)
			{
				this.buffer = buff;
				this.position = count;
			}
			else
			{
				// if the array isn't big enough, make it bigger
				while (buffer.Length < position + count)
				{
					buffer = new byte[buffer.Length*2];
				}
				System.Buffer.BlockCopy(buff, offset, buffer, (int) position, count);
				position += count;
			}

		}

	}
}