using System;
using System.Collections;
using System.Web;

namespace SiteMesh.Mapper
{

	/// <summary>
	/// The CookieDecoratorMapper will map a suitable decorator based on a cookie value.
	///
	/// <p>The CookieDecoratorMapper is configured via one properties.
	/// <code>cookie.name</code> - the cookie which contains the name of the
	/// decorator which will be mapped.</p>
	///
	/// @author Paul Hammant
	/// @version $Revision: 1.2 $
	/// </summary>
	public class CookieDecoratorMapper : AbstractDecoratorMapper
	{
		private string cookieName;

		public override void Init(IDictionary properties, IDecoratorMapper parent)
		{
			base.Init(properties, parent);
			cookieName = (string) properties["cookie.name"];
			if (cookieName == null)
			{
				throw new Exception("'cookie.name' name parameter not set for this decorator mapper");
			}
		}

		public override IDecorator GetDecorator(HttpRequest request, IPage page)
		{
			IDecorator result = null;

			// loop through request cookies and try to find match
			foreach (System.Web.HttpCookie cookie in request.Cookies) {

				if (cookie.Name.Equals(cookieName)) {
                    result = GetNamedDecorator(request, cookie.Value);
                }
            }
			return result == null ? base.GetDecorator(request, page) : result;
		}
	}

}