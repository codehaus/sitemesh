using System.Collections;

namespace SiteMesh.Mapper
{

	/// <summary>
	/// Default implementation of Decorator. All properties are set by the
	/// constructor.
	///
	/// @author <a href="mailto:joe@truemesh.com">Joe Walnes</a>
	/// @version $Revision: 1.1 $ 
	/// </summary>
	public class DefaultDecorator : IDecorator
	{
		/** @see #getPage() */
		protected string page = null;

		/** @see #getName() */
		protected string name = null;

		/** @see #getURIPath() */
		protected string uriPath = null;

		/** @see #getRole() */
		protected string role = null;

		/** @see #getInitParameter(java.lang.String) */
		protected IDictionary parameters = null;

		/// <summary>
		/// Constructor to set name, page and parameters.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="page"></param>
		/// <param name="parameters"></param>
		public DefaultDecorator(string name, string page, IDictionary parameters) : this(name, page, null, null, parameters)
		{
		}

		/// <summary>
		/// Constructor to set all properties.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="page"></param>
		/// <param name="uriPath"></param>
		/// <param name="parameters"></param>
		public DefaultDecorator(string name, string page, string uriPath, IDictionary parameters) : this(name, page, uriPath, null, parameters)
		{
		}

		/// <summary>
		/// Constructor to set all properties.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="page"></param>
		/// <param name="uriPath"></param>
		/// <param name="role"></param>
		/// <param name="parameters"></param>
		public DefaultDecorator(string name, string page, string uriPath, string role, IDictionary parameters)
		{
			this.name = name;
			this.page = page;
			this.uriPath = uriPath;
			this.role = role;
			this.parameters = parameters;
		}

		/// <summary>
		/// URI of the page to dispatch the request to (relative to the
		/// application root). 
		/// </summary>
		public string Page
		{
			get { return page; }
		}

		/// <summary>
		/// Name of Decorator. For information purposes only.
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// URI path of the Decorator. Enables support for decorators defined in seperate web-apps.
		/// </summary>
		public string UriPath
		{
			get { return uriPath; }
		}

		/// <summary>
		/// Role the user has to be in to get this decorator applied.
		/// </summary>
		public string Role
		{
			get { return role; }
		}

		/// <summary>
		/// Returns a String containing the value of the named initialization parameter,
		/// or null if the parameter does not exist. 
		/// </summary>
		/// <param name="paramName">Key of parameter.</param>
		/// <returns>Value of parameter or null if not found.</returns>
		public virtual string GetInitParameter(string paramName)
		{
			if (parameters == null || !parameters.Contains(paramName))
			{
				return null;
			}

			return (string) parameters[paramName];
		}

		/// <summary>
		/// Returns the names of the Decorator's initialization parameters as an Iterator
		/// of String objects, or an empty Iterator if the Decorator has no initialization parameters. 
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetInitParameterNames()
		{
			if (parameters == null)
			{
				// make sure we always return an empty iterator
				return new Hashtable().GetEnumerator();
			}

			return parameters.Keys.GetEnumerator();
		}
	}
}