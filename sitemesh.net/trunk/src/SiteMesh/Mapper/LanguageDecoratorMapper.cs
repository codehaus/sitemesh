using System;
using System.Collections;
using System.IO;
using System.Web;

namespace SiteMesh.Mapper
{
	/// <summary>
	/// The LanguageDecoratorMapper can determine the preferred language set in the
	/// browser requesting a page, and map to a suitable Decorator (using the
	/// "Accept-Language" HTTP header).
	///
	/// <p>This can be useful for supplying different versions of the same content
	/// for different languages.</p>
	///
	/// <p>When LanguageDecoratorMapper is in the chain, it will request the appropriate Decorator
	/// from its parent. It will then add an extention to the filename of the Decorator, and
	/// if that file exists it shall be used as the Decorator instead. For example, if the
	/// Decorator path is <code>/blah.jsp</code> and the detected preferred language is <code>en</code>,
	/// the path <code>/blah-en.jsp</code> shall be used.</p>
	///
	/// <p>The language mappings are configured by passing properties with <code>match.</code> as a prefix.
	/// For example: 'match.en'=engl , 'match.nl'=dutch .</p>
	///
	/// @author <a href="mailto:pathos@pandora.be">Mathias Bogaert</a>
	/// </summary>
	public sealed class LanguageDecoratorMapper : AbstractDecoratorMapper 
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
				string path = ModifyPath(d.Page, GetExt(request.Headers["Accept-Language"]));

				//File decFile = null; //new File(request.Server.MapPath(path));

				/*if (decFile.isFile()) {
					result = new DefaultDecorator(d.getName(), path, null) {
						public string getInitParameter(string paramName) {
							return d.getInitParameter(paramName);
						}
					};
				}*/
				return result == null ? base.GetDecorator(request, page) : result;
			}
			catch 
			{
				return base.GetDecorator(request, page);
			}
		}

		/** Get extention for the language. */
		private string GetExt(string acceptLanguage) 
		{
			foreach (string key in map.Keys) {

			    // Get the first language (preferred one) in the header, and
				// only check the first two chars (the acceptLanguage could be en-gb, but
				// we don't support this for now).
				if (acceptLanguage.Substring(0, 2).Equals(key)) {
					return (string) map[key];
				}
			

				// When the user-agent has multiple accept-languages (separated by ;),
				// these are ignored because the site creator may wish that if the
				// preferred language is not supported, the page uses the default
				// decorator (in the default language), and not in some other
				// language given by the browser (most of them are specified by
				// default at install).
			}
			return null;
		}

		/** Change /abc/def.jsp into /abc/def-XYZ.jsp */
		private static string ModifyPath(String path, String ext) 
		{
			int dot = path.IndexOf('.');
			if (dot > -1) 
			{
				return path.Substring(0, dot) + '-' + ext + path.Substring(dot);
			}
			else 
			{
				return path + '-' + ext;
			}
		}

		/** Initialize language mappings. */
		private void InitMap(IDictionary props) 
		{
			ICollection keys = props.Keys;
			foreach (string key in keys) {

				if (key.StartsWith("match.")) {
					string match = key.Substring(6);
					map.Add(match, props[key]);
				}
			}
		}
	}
}