namespace SiteMesh
{
	/// <summary>
	/// The PageParser is responsible for parsing the page data into an appropriate
	/// {@link com.opensymphony.module.sitemesh.Page} object.
	///
	/// <p>The implementation of this can be switched to parse different kind of data
	/// (<i>e.g.</i> HTML, WML, FOP, images) or for performance enhancements. An
	/// implementation is obtained through the {@link com.opensymphony.module.sitemesh.Factory} .</p>
	///
	/// <p>A single PageParser is reused, therefore the parse() methods need to be thread-safe.</p>
	///
	/// @author <a href="joe@truemesh.com">Joe Walnes</a> 
	/// </summary>
	public interface IPageParser
	{
		IPage Parse(byte[] data);
		IPage Parse(byte[] data, string encoding);
	}

}