using System;
using System.Collections;
using System.Web;

namespace SiteMesh
{
	/// <summary>
	/// The DecoratorMapper is responsible for determining which
	/// {@link com.opensymphony.module.sitemesh.Decorator} should be used for a
	/// {@link com.opensymphony.module.sitemesh.Page}.
	///
	/// <p>Implementations of this are returned by the {@link com.opensymphony.module.sitemesh.Factory},
	/// and should be thread-safe.</p>
	///
	/// @author <a href="mailto:joe@truemesh.com">Joe Walnes</a>
	/// @version $Revision: 1.1 $ 
	/// </summary>
	public interface IDecoratorMapper
	{
		/// <summary>
		/// Initialize the mapper. This is always called before the other methods. 
		/// </summary>
		/// <param name="properties">Any initialization properties (specific to implementation).</param>
		/// <param name="parent">Parent mapper for this mapper</param>
		void Init(IDictionary properties, IDecoratorMapper parent);

		/**
		 * 
		 *
		 * @associates Decorator
		 * @label maps to
		 */

		/// <summary>
		/// Return appropriate IDecorator for a certain Page.
		///
		/// <p>The implementation can determine the result based on the actual request
		/// or the data of the parsed page. Typically this would call <code>GetNamedDecorator()</code>
		/// which would delegate to a parent DecoratorMapper.</p>
		/// </summary>
		/// <param name="request">HTTP request</param>
		/// <param name="page">Page requested</param>
		/// <returns>Suitable IDecorator instance</returns>
		IDecorator GetDecorator(HttpRequest request, IPage page);

		/// <summary>
		/// Return a IDecorator with given name.
		/// </summary>
		/// <param name="request">HTTP request</param>
		/// <param name="name">Decorator name</param>
		/// <returns>IDecorator</returns>
		IDecorator GetNamedDecorator(HttpRequest request, string name);
	}

}