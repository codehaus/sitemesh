using System;
using System.Collections;
using System.Web;

namespace SiteMesh.Mapper
{
	/// <summary>
	/// The AgentDecoratorMapper can determine the user-agent (i.e. web-browser)
	/// requesting a page, and map to a suitable Decorator.
	///
	/// <p>This can be useful for supplying different versions of the same content
	/// for different browsers (e.g. vanilla HTML for Lynx, complex tables and frames
	/// for Netscape, extra stuff for IE5, etc).</p>
	///
	/// <p>This can also be used to enhance search-engine ratings by using a 'bait and
	/// switch' system - this involves showing a search-engine friendly of the content
	/// to spiders only.</p>
	///
	/// <p>When AgentDecoratorMapper is in the chain, it will request the appropriate Decorator
	/// from its parent. It will then add an extention to the filename of the Decorator, and
	/// if that file exists it shall be used as the Decorator instead. For example, if the
	/// Decorator path is <code>/blah.jsp</code> and the detected user-agent is <code>ie</code>,
	/// the path <code>/blah-ie.jsp</code> shall be used.</p>
	///
	/// <p>The agent mappings are configured by passing properties with <code>match.</code> as a prefix.
	/// For example: 'match.MSIE'=ie , 'match.Lynx'=plain .</p>
	/// 
	/// @author <a href="mailto:joe@truemesh.com">Joe Walnes</a>
	/// </summary>
	public sealed class AgentDecoratorMapper : AbstractDecoratorMapper
	{
		private IDictionary map = null;

		public override void Init(IDictionary properties, IDecoratorMapper parent)
		{
			base.Init(properties, parent);
			map = new Hashtable();
			InitMap(properties);
		}

		public override IDecorator GetDecorator(HttpRequest request, IPage page)
		{
			try
			{
				IDecorator result = null;
				IDecorator d = base.GetDecorator(request, page);
				string path = ModifyPath(d.Page, GetExt(request.Headers["User-Agent"]));

				/*            File decFile = new File(HttpContext.Current.(path));

            if (decFile.isFile()) 
			{
				result = new FileMatchDecorator(path, d);
            }
			*/
				return result == null ? base.GetDecorator(request, page) : result;
			}
			catch (NullReferenceException)
			{
				return base.GetDecorator(request, page);
			}
		}

		/** Get extention for user-agent. */

		private string GetExt(String userAgent)
		{
			/*Iterator i = map.entrySet().iterator();
        while (i.hasNext()) {
            Map.Entry entry = (Map.Entry) i.next();
            String curr = (String) entry.getKey();
            if (userAgent.indexOf(curr) > -1) return (String) entry.getValue();
        }'*/
			return null;

		}

		/** Change /abc/def.jsp into /abc/def-XYZ.jsp */

		private static string ModifyPath(String path, String ext)
		{
			/*
		int dot = path.indexOf('.');
        if (dot > -1) {
            return path.substring(0, dot) + '-' + ext + path.substring(dot);
        }
        else {
            return path + '-' + ext;
        }
		*/
			return null;
		}

		/** Initialize user-agent mappings. */

		private void InitMap(IDictionary props)
		{
			/*IEnumerator i = props.
        while (i.hasNext()) {
            Map.Entry entry = (Map.Entry) i.next();
            String key = (String) entry.getKey();
            if (key.startsWith("match.")) {
                String match = key.substring(6);
                String ext = (String) entry.getValue();
                map.put(match, ext);
            }
        }
		*/
		}
	}

	internal class FileMatchDecorator : DefaultDecorator
	{
		private IDecorator decorator = null;


		public FileMatchDecorator(string path, IDecorator d) : base(d.Name, path, null)
		{
			decorator = d;
		}

		public override string GetInitParameter(String paramName)
		{
			return decorator.GetInitParameter(paramName);
		}
	}
}