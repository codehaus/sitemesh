using System;
using System.Collections;
using System.Web;

namespace SiteMesh.Mapper
{
	/// <summary>
	/// The FrameSetDecoratorMapper will use the specified decorator when the Page
	/// is an instance of {@link com.opensymphony.module.sitemesh.HTMLPage} and
	/// <code>isFrameSet()</code> returns true.
	///
	/// <p>The name of this decorator should be supplied in the <code>decorator</code>
	/// property - if no decorator property is supplied, no decorator is applied to
	/// frame based pages.
	///
	/// @author <a href="mailto:joe@truemesh.com">Joe Walnes</a>
	/// @version $Revision: 1.1 $
	/// </summary>
	public class FrameSetDecoratorMapper : AbstractDecoratorMapper 
	{
		private string decorator = null;

		public override void Init(IDictionary properties, IDecoratorMapper parent) 
		{
			base.Init(properties, parent);
			decorator = properties["decorator"] as string;
		}

		public override IDecorator GetDecorator(HttpRequest request, IPage page) 
		{
			IHtmlPage htmlPage = page as IHtmlPage;
		
			if (htmlPage != null && htmlPage.FrameSet) 
			{
				return GetNamedDecorator(request, decorator);
			}
			else 
			{
				return base.GetDecorator(request, page);
			}
		}
	}
}