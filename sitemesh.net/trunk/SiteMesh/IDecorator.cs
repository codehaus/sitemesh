using System.Collections;

namespace SiteMesh
{
	/// <summary>
	/// Representation of a Decorator.
	/// 
	/// <p>A Decorator is infact a ASPX page, and this is a wrapper to reference it.
	/// An implementation is returned by the Sitemesh.DecoratorMapper.</p>
	/// </summary>
	public interface IDecorator
	{
		/// <summary>
		/// URI of the Servlet/JSP to dispatch the request to (relative to the
		/// web-app context).
		/// </summary>
		string Page { get; }

		/// <summary>
		/// Name of the Decorator. For informational purposes only.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// URI path of the Decorator. Enables support for decorators defined in seperate web-apps.
		/// </summary>
		string UriPath { get; }

		/// <summary>
		/// Role the user has to be in to get this decorator applied.
		/// </summary>
		string Role { get; }

		/// <summary>
		/// Returns a String containing the value of the named initialization parameter,
		/// or null if the parameter does not exist. 
		/// </summary>
		/// <param name="paramName">Key of parameter.</param>
		/// <returns>Value of the parameter or null if not found.</returns>
		string GetInitParameter(string paramName);

		/// <summary>
		/// Returns the names of the Decorator's initialization parameters as an Iterator
		/// of String objects, or an empty Iterator if the Decorator has no initialization parameters.
		/// </summary>
		/// <returns></returns>
		IEnumerator GetInitParameterNames();
	}
}