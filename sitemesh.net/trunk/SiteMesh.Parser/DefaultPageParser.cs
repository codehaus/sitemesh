
namespace SiteMesh.Parser
{
	/// <summary>
	/// Default implementation of PageParser - returns an UnParsedPage.
	///
	/// @author <a href="joe@truemesh.com">Joe Walnes</a>
	/// @version $Revision: 1.1 $
	/// </summary>
	public class DefaultPageParser : IPageParser
	{
		public IPage Parse(byte[] data)
		{
			return new UnParsedPage(data);
		}

		public IPage Parse(byte[] data, string encoding)
		{
			return new UnParsedPage(data);
		}
	}
}