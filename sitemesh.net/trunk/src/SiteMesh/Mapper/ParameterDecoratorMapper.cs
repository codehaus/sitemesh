using System.Collections;
using System.Web;

namespace SiteMesh.Mapper
{
	/// <summary>
	/// The ParameterDecoratorMapper will map a suitable decorator based on request
	/// parameters.
	///
	/// <p>The ParameterDecoratorMapper is configured via three properties.
	/// <code>decorator.parameter</code> - the parameter which contains the name of the decorator which will be mapped.
	/// The default is "decorator".</p>
	///
	/// <p>For example if <code>decorator.parameter</code> is "foobar" then
	/// myurl.jsp?foobar=mydecorator will map to the decorator named "mydecorator".</p>
	///
	/// <p>You can also supply an optional 'confirmation parameter'.
	/// The decorator will only be mapped if the parameter named <code>parameter.name</code> is
	/// in the request URI and the value of that parameter is equal to the
	/// <code>parameter.value</code> property.</p>
	///
	/// <p>For example assuming parameter.name=confirm and parameter.value=true
	/// the URI myurl.jsp?decorator=mydecorator&confirm=true will map the decorator mydecorator.
	/// where as the URIs myurl.jsp?decorator=mydecorator and myurl.jsp?decorator=mydecorator&confirm=false will
	/// not return any decorator.</p>
	///
	/// @author <a href="mailto:mcannon@internet.com">Mike Cannon-Brookes</a>
	/// @version $Revision: 1.2 $
	/// </summary>
	public class ParameterDecoratorMapper : AbstractDecoratorMapper
	{
		private string decoratorParameter = null, paramName = null, paramValue = null;

		public override void Init(IDictionary properties, IDecoratorMapper parent)
		{
			base.Init(properties, parent);

			decoratorParameter = (string) properties["decorator.parameter"];
			if (decoratorParameter == null)
			{
				decoratorParameter = "decorator";
			}
			paramName = (string) properties["parameter.name"];
			paramValue = (string) properties["parameter.value"];
		}

		public override IDecorator GetDecorator(HttpRequest request, IPage page)
		{
			IDecorator result = null;
			string decoratorParamValue = request.Params[decoratorParameter];

			if ((paramName == null || paramValue == request.Params[paramName])
				&& decoratorParamValue != null && decoratorParamValue.Trim() != string.Empty)
			{
				result = GetNamedDecorator(request, decoratorParamValue);
			}
			return result == null ? base.GetDecorator(request, page) : result;
		}
	}
}