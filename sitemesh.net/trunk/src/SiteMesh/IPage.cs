using System.Collections;
using System.IO;
using System.Web;

namespace SiteMesh
{
	/// <summary>
	/// The Page object wraps the contents of the original (undecorated) page.
	/// 
	/// <p>The original data in its entirity can be written using the writePage()
	/// methods. It may also contain a set of properties - these vary among
	/// different {@link com.opensymphony.module.sitemesh.PageParser} implementations.</p>
	/// 
	/// <p>Typically a Page is no use to a {@link com.opensymphony.module.sitemesh.Decorator} as it needs
	/// specific details relevant to the content-type of that page (<i>e.g.</i> HTML
	/// pages). The appropriate {@link com.opensymphony.module.sitemesh.PageParser} is responsible
	/// for returning extended implementations of pages such as {@link com.opensymphony.module.sitemesh.HTMLPage}
	/// which are of more use to the Decorator. New media types (<i>e.g.</i> WML) could
	/// be added to the system by extending Page and implementing an appropriate PageParser.</p>
	/// 
	/// @author <a href="mailto:joe@truemesh.com">Joe Walnes</a>
	/// </summary>
	public interface IPage
	{
		/// <summary>
		/// Write the entire contents of the <code>Page</code>, in the format before
		/// it was parsed, to the <code>Writer</code>. 
		/// </summary>
		/// <param name="outStream">Stream to write to.</param>
		void WritePage(Stream outStream);

		/// <summary>
		/// Write the contents of the <code>&lt;body&gt;</code> tag. 
		/// </summary>
		/// <param name="outWriter">Writer to write to.</param>
		void WriteBody(TextWriter outWriter);

		/// <summary>
		/// Get the Title of the document
		/// </summary>
		string Title { get; }


		string Body { get; }

		/// <summary>
		/// Get whole head section of document.
		/// </summary>
		string Head { get; }

		/// <summary>
		/// Length of the <code>Page</code>, in the format before
		/// it was parsed.
		/// 
		/// <returns> Length of page data (in number of bytes).</returns>
		/// </summary>
		int ContentLength { get; }

		/// <summary>
		/// Get a property embedded into the <code>Page</code> as a <code>String</code>. 
		/// </summary>
		/// <param name="name">Name of property</param>
		/// <returns>Property value</returns>
		string GetProperty(string name);

		/// <summary>
		/// Get a property embedded into the <code>Page</code> as an <code>int</code>.
		/// Returns 0 if property not specified or not valid number. 
		/// </summary>
		/// <param name="name">Name of property</param>
		/// <returns>Property value</returns>
		int GetIntProperty(string name);

		/// <summary>
		/// Get a property embedded into the <code>Page</code> as a <code>long</code>.
		/// Returns 0L if property not specified or not valid number.
		/// </summary>
		/// <param name="name">Name of property</param>
		/// <returns>Property value</returns>
		long GetLongProperty(string name);

		/// <summary>
		/// Get a property embedded into the <code>Page</code> as a <code>boolean</code>.
		/// Returns true if value starts with '1', 't' or 'y' (case-insensitive) -
		/// otherwise returns false.
		/// </summary>
		/// <param name="name">Name of property</param>
		/// <returns>Property value</returns>
		bool GetBooleanProperty(string name);

		/// <summary>
		/// Determine whether a property embedded into the <code>Page</code> has been set.
		/// </summary>
		/// <param name="name">Name of property</param>
		/// <returns>Property value</returns>
		bool PropertySet(string name);

		/// <summary>
		/// Get all available property keys for the <code>Page</code>.
		/// </summary>
		/// <returns>Property keys</returns>
		string[] PropertyKeys { get; }

		/// <summary>
		/// Get a <code>Map</code> representing all the properties in the <code>Page</code>.
		/// </summary>
		/// <returns>Properties map</returns>
		IDictionary Properties { get; }

		/// <summary>
		/// Return the request of the original page.
		/// </summary>
		HttpRequest Request { get; set; }

		/// <summary>
		/// Manually add a property to page.
		/// </summary>
		/// <param name="name">Name of the property</param>
		/// <param name="data">Data to associate with the name</param>
		void AddProperty(string name, string data);
	}

}