using System.Configuration;
using System.Xml;

namespace SiteMesh.Factory
{
	public class PageParsersSectionHandler : IConfigurationSectionHandler
	{
		private XmlNode section;

		public object Create(object parent, object configContext, XmlNode section)
		{
			this.section = section;
			return this;
		}

		public XmlNode Section
		{
			get { return section; }
		}
	}
}