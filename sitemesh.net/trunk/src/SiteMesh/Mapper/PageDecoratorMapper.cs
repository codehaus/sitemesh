using System;
using System.Collections;
using System.Web;

namespace SiteMesh.Mapper
{
	/// <summary>
	/// The PageDecoratorMapper allows the actual Page to determine the Decorator to be
	/// used.
	///
	/// <p>The 'meta.decorator' and 'decorator' properties of the page are accessed
	/// and if any of them contain the name of a valid Decorator, that Decorator shall
	/// be applied.</p>
	///
	/// <p>As an example, if HTML is being used, the Decorator could be chosen by using
	/// a <code>&lt;html decorator="mydecorator"&gt;</code> root tag <i>or</i> by using a
	/// <code>&lt;meta name="decorator" content="mydecorator"&gt;</code> tag in the header.</p>
	///
	/// <p>The actual properties to query are specified by passing properties to the mapper using the
	/// <code>property.?</code> prefix. As the properties are stored in a Map, each key has to be unique.
	/// Example: property.1=decorator, property.2=meta.decorator .</p>
	///
	/// @author <a href="mailto:joe@truemesh.com">Joe Walnes</a>
	/// @version $Revision: 1.2 $
	/// </summary>
	public class PageDecoratorMapper : AbstractDecoratorMapper 
	{
		private IList pageProps = null;

		public override void Init(IDictionary properties, IDecoratorMapper parent) 
		{
			base.Init(properties, parent);
			pageProps = new ArrayList();
		
			foreach(DictionaryEntry entry in properties) 
			{
				string key = (string) entry.Key;
				if (key.StartsWith("property")) 
				{
					pageProps.Add(entry.Value);
				}
			}
		}

		public override IDecorator GetDecorator(HttpRequest request, IPage page) 
		{
			IDecorator result = null;
		
			foreach (string propName in pageProps) 
			{
				result = GetByProperty(request, page, propName);
				if (result != null) break;
			}
			return result == null ? base.GetDecorator(request, page) : result;
		}

		private IDecorator GetByProperty(HttpRequest request, IPage p, string name) 
		{
			if (p.PropertySet(name)) 
			{
				return GetNamedDecorator(request, (string) p.Properties[name]);
			}
			return null;
		}
	}
}