using System;
using System.Collections;
using System.Configuration;
using System.Web;

namespace SiteMesh.Mapper
{
	/// <summary>
	/// Default implementation of DecoratorMapper. Reads decorators and
	/// mappings from the <code>config</code> property (default '/WEB-INF/decorators.xml').
	///
	/// @author <a href="joe@truemesh.com">Joe Walnes</a>
	/// @author <a href="mcannon@internet.com">Mike Cannon-Brookes</a>
	/// @version $Revision: 1.1 $
	/// </summary>
	public class ConfigDecoratorMapper : AbstractDecoratorMapper
	{
		private DecoratorSectionHandler decoratorConfig = null;

		/// <summary>
		/// Create new ConfigLoader using 'web.config' file.
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="parent"></param>
		public override void Init(IDictionary properties, IDecoratorMapper parent)
		{
			base.Init(properties, parent);
			decoratorConfig = (DecoratorSectionHandler) ConfigurationSettings.GetConfig("sitemesh/decorators");
		}

		/// <summary>
		/// Retrieve Decorator based on 'pattern' tag.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="currentPage"></param>
		/// <returns></returns>
		public override IDecorator GetDecorator(HttpRequest request, IPage currentPage)
		{
			// ask the decorator config for a named decorator mapped to the path of the request
			IDecorator result = null;

			string thisPath = request.Path;

			if (thisPath == null)
			{
				Uri requestUri = request.Url;
			}
			string name = decoratorConfig.GetMappedName(thisPath);
			result = GetNamedDecorator(request, name);

			// otherwise ask the parent
			if (result == null)
			{
				result = base.GetDecorator(request, currentPage);
			}

			return result;
		}

		/** Retrieve Decorator named in 'name' attribute. Checks the role if specified. */

		public override IDecorator GetNamedDecorator(HttpRequest request, string name)
		{
			IDecorator result = null;
			result = decoratorConfig.GetDecoratorByName(name);
			if (result == null || (result.Role != null && !HttpContext.Current.User.IsInRole(result.Role)))
			{
				// if the result is null or the user is not in the role
				return base.GetNamedDecorator(request, name);
			}
			else
			{
				return result;
			}
		}
	}
}