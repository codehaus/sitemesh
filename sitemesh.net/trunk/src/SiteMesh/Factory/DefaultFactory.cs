using System;
using System.Collections;
using System.Configuration;
using System.Globalization;
using System.Xml;

namespace SiteMesh.Factory
{
	/// <summary>
	/// DefaultFactory, reads configuration from <code>/WEB-INF/sitemesh.xml</code>, or uses the
	/// default configuration if <code>sitemesh.xml</code> does not exist.
	///
	/// @author <a href="mailto:joe@truemesh.com">Joe Walnes</a>
	/// @author <a href="mailto:pathos@pandora.be">Mathias Bogaert</a>
	/// @version $Revision: 1.1 $
	/// </summary>
	public class DefaultFactory : BaseFactory
	{
		public DefaultFactory() : base()
		{
			LoadConfig();
		}

		/// <summary>
		/// Refresh config before delegating to superclass.
		/// </summary>
		/// <returns></returns>
		public IDecoratorMapper getDecoratorMapper()
		{
			return base.GetDecoratorMapper();
		}

		/// <summary>
		/// Refresh config before delegating to superclass.
		/// </summary>
		/// <param name="contentType"></param>
		/// <returns></returns>
		public override IPageParser GetPageParser(string contentType)
		{
			return base.GetPageParser(contentType);
		}

		/// <summary>
		/// Refresh config before delegating to superclass.
		/// </summary>
		/// <param name="contentType">Content type of page.</param>
		/// <returns></returns>
		public override bool ShouldParsePage(string contentType)
		{
			return base.ShouldParsePage(contentType);
		}

		/// <summary>
		/// Load configuration from web.config.
		/// </summary>
		private void LoadConfig()
		{
			lock (this)
			{
				try
				{
					DecoratorMappersSectionHandler mapperSectionHandler = (DecoratorMappersSectionHandler) ConfigurationSettings.GetConfig("sitemesh/decorator-mappers");
					LoadDecoratorMappers(mapperSectionHandler.Section);
					PageParsersSectionHandler parserSectionHandler = (PageParsersSectionHandler) ConfigurationSettings.GetConfig("sitemesh/page-parsers");
					LoadPageParsers(parserSectionHandler.Section);
				}
				catch (Exception e)
				{
					Report("Could not parse web.config file", e);
				}
			}
		}

		/// <summary>
		/// Loop through children of 'page-parsers' element and add all 'parser' mappings.
		/// </summary>
		/// <param name="parserSectionHandler"></param>
		private void LoadPageParsers(XmlNode parserSectionHandler)
		{
			ClearParserMappings();

			XmlNodeList nodes = parserSectionHandler.ChildNodes;
			XmlElement curr = null;
			for (int i = 0; i < nodes.Count; ++i)
			{
				curr = nodes.Item(i) as XmlElement;
				if (curr != null)
				{
					if (curr.Name.ToLower(CultureInfo.InvariantCulture) == "parser")
					{
						string typeName = curr.GetAttribute("type");
						string contentType = curr.GetAttribute("content-type");
						MapParser(contentType, typeName);
					}
				}
			}
		}

		/// <summary>
		/// Loop through children of 'decorator-mappers' element and add all 'mapper' mappings.
		/// </summary>
		/// <param name="mapperSection"></param>
		private void LoadDecoratorMappers(XmlNode mapperSection)
		{
			ClearDecoratorMappers();
			IDictionary emptyProps = new Hashtable();

			PushDecoratorMapper("SiteMesh.Mapper.NullDecoratorMapper, SiteMesh", emptyProps);

			// note, this works from the bottom node up.

			XmlNodeList nodes = mapperSection.ChildNodes;
			XmlElement curr = null;

			for (int i = nodes.Count; i >= 0; --i)
			{
				curr = nodes.Item(i) as XmlElement;

				if (curr == null)
				{
					// we only do xml elements
					continue;
				}

				if (curr.Name.ToLower(CultureInfo.InvariantCulture) == "mapper")
				{
					string typeName = curr.GetAttribute("type");
					Hashtable props = new Hashtable();
					XmlNodeList children = curr.ChildNodes;
					XmlElement currC = null;

					for (int j = 0; j < children.Count; ++j)
					{
						currC = children[j] as XmlElement;
						if (currC != null)
						{
							if (currC.Name.ToLower(CultureInfo.InvariantCulture) == "param")
							{
								props.Add(currC.GetAttribute("name"), currC.GetAttribute("value"));
							}
						}
					}
					PushDecoratorMapper(typeName, props);
				}
			}
			PushDecoratorMapper("SiteMesh.Mapper.InlineDecoratorMapper, SiteMesh", emptyProps);
		}
	}
}