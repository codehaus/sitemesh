using System;
using System.Web;

namespace SiteMesh.Mapper
{
	/// <summary>
	/// The InlineDecoratorMapper is used to determine the correct Decorator when
	/// using inline decorators.
	///
	/// <p>It will check the request attribute value defined by the key
	/// Sitemesh.SiteMeshConstants.DECORATOR and use the appropriate named
	/// Decorator. This is passed across from the page:applyDecorator tag.</p>
	///
	/// @author <a href="mailto:joe@truemesh.com">Joe Walnes</a>
	/// </summary>
	public class InlineDecoratorMapper : AbstractDecoratorMapper
	{
		public override IDecorator GetDecorator(HttpRequest request, IPage page)
		{
			IDecorator result = null;

			if (request.Params[SiteMeshConstants.DECORATOR] != null)
			{
				if (request.Params[SiteMeshConstants.DECORATOR] != null)
				{
					// Retrieve name of decorator to use from request
					string decoratorName = (string) request.Params[SiteMeshConstants.DECORATOR];
					result = GetNamedDecorator(request, decoratorName);
					if (result == null)
					{
						throw new Exception("Cannot locate inline Decorator: " + decoratorName);
					}
				}
				return result == null ? base.GetDecorator(request, page) : result;
			}

			return result;
		}
	}
}