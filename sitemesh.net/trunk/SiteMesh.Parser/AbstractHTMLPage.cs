

using System.IO;

namespace SiteMesh.Parser
{
	/// <summary>
	/// Abstract implementation of SiteMesh.IHTMLPage.
	/// 
	/// <p>Adds to SiteMesh.Parser.AbstractPage some HTML methods.
	///	To implement, follow guidelines of super-class, and implement the 2
	/// abstract methods states below.</p>
	/// </summary>
	public abstract class AbstractHTMLPage : AbstractPage, IHtmlPage
	{
		/// <summary>
		/// Write data of html <code>&lt;head&gt;</code> tag.
		/// <p>Must be implemented. Data written should not actually contain the
		/// head tags, but all the data in between.
		/// </summary>
		/// <param name="outWriter">Writer to write to</param>
		public abstract void WriteHead(TextWriter outWriter);

		/// <summary>
		/// Is page a frameset page.
		/// </summary>
		public abstract bool FrameSet { get; }

	}
}