using System.IO;

namespace SiteMesh.Parser
{
	/// <summary>
	/// A page that is of unrecognised content-type, or cannot be parsed into
	/// a specific type of Page.
	///
	/// <p>The original page is contained within, but no meta-data or parsed chunks.</p>
	///
	/// @author <a href="joe@truemesh.com">Joe Walnes</a>
	/// </summary>
	public sealed class UnParsedPage : AbstractPage
	{
		/// <summary>
		/// Simple constructor.
		/// </summary>
		/// <param name="original">Original data of page.</param>
		public UnParsedPage(byte[] original)
		{
			this.pageData = original;
		}

		/// <summary>
		/// Do-nothing implementation.
		/// </summary>
		/// <param name="outWriter">Writer to write to</param>
		public override void WriteBody(TextWriter outWriter)
		{
		}
	}
}