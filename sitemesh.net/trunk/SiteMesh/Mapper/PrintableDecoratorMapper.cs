using System.Collections;
using System.Web;

namespace SiteMesh.Mapper
{
	/// <summary>
	/// The PrintableDecoratorMapper is a sample DecoratorMapper that will
	/// check to see whether 'printable=true' is supplied as a request parameter
	/// and if so, use the specified decorator instead. The name of this decorator
	/// should be supplied in the <code>decorator</code> property.
	/// 
	/// p>The exact 'printable=true' request criteria can be overriden with the
	/// <code>parameter.name</code> and <code>parameter.value</code> properties.</p>
	/// 
	/// <p>Although this DecoratorMapper was designed for creating printable versions
	/// of a page, it can be used for much more imaginative purposes.</p>
	/// 
	/// @author <a href="mailto:joe@truemesh.com">Joe Walnes</a>
	/// </summary>
	public sealed class PrintableDecoratorMapper : AbstractDecoratorMapper
	{
		private string decorator;
		private string paramName;
		private string paramValue;

		public override void Init(IDictionary properties, IDecoratorMapper parent)
		{
			base.Init(properties, parent);
			decorator = (string) properties["decorator"];
			paramName = (string) properties["parameter.name"];
			if (paramName == null)
			{
				paramName = "printable";
			}
			paramValue = (string) properties["parameter.value"];
			if (paramValue == null)
			{
				paramValue = "true";
			}
		}

		public override IDecorator GetDecorator(HttpRequest request, IPage currentPage)
		{
			IDecorator result = null;

			if (decorator != null && paramValue.ToLower().Equals(request.Params.Get(paramName)))
			{
				result = GetNamedDecorator(request, decorator);
			}

			if (result == null)
			{
				return base.GetDecorator(request, currentPage);
			}

			return result;
		}

	}
}