using System.Collections;
using System.Configuration;
using System.Xml;

namespace SiteMesh.Mapper
{
	public class DecoratorSectionHandler : IConfigurationSectionHandler
	{
		private IDictionary decorators = null;
		private PathMapper pathMapper = null;

		public object Create(object parent, object configContext, XmlNode section)
		{
			ParseConfig(section);
			return this;
		}

		public string GetMappedName(string path)
		{
			return pathMapper.Item(path);
		}

		public IDecorator GetDecoratorByName(string name)
		{
			if (name == null || name == string.Empty)
			{
				return null;
			}
			else
			{
				return (IDecorator) decorators[name];
			}
		}


		private void ParseConfig(XmlNode section)
		{
			XmlElement root = (XmlElement) section;

			// get the default directory for the decorators
			string defaultDir = GetAttribute(root, "defaultdir");

			if (defaultDir == null)
			{
				defaultDir = GetAttribute(root, "defaultDir");
			}

			// clear previous config
			pathMapper = new PathMapper();
			decorators = new Hashtable();

			// get decorators
			XmlNodeList decoratorNodes = root.GetElementsByTagName("decorator");
			string name, page, uriPath, role;

			foreach (XmlElement decoratorElement in decoratorNodes)
			{
				name = GetAttribute(decoratorElement, "name");
				page = GetAttribute(decoratorElement, "page");
				uriPath = GetAttribute(decoratorElement, "webapp");
				role = GetAttribute(decoratorElement, "role");

				// append the defaultDir
				if (defaultDir != null && page != null && page.Length > 0)
				{
					if (page[0] == '/')
					{
						page = defaultDir + page;
					}
					else
					{
						page = defaultDir + "/" + page;
					}
				}

				// the uriPath must begin with a slash
				if (uriPath != null && uriPath.Length > 0)
				{
					if (uriPath[0] != '/')
					{
						uriPath = "/" + uriPath;
					}
				}

				// get all <pattern>...</pattern> nodes and add a mapping
				XmlNodeList patternNodes = decoratorElement.GetElementsByTagName("pattern");

				foreach (XmlElement p in patternNodes)
				{
					string pattern = ((XmlText) p.FirstChild).Data;
					if (role != null)
					{
						pathMapper.Add(name + role, pattern);
					}
					else
					{
						pathMapper.Add(name, pattern);
					}
				}

				IDictionary parameters = new Hashtable();
				XmlNodeList paramNodes = decoratorElement.GetElementsByTagName("init-param");

				foreach (XmlElement paramElement in paramNodes)
				{
					string paramName = GetContainedText(paramElement, "param-name");
					string paramValue = GetContainedText(paramElement, "param-value");
					parameters.Add(paramName, paramValue);
				}

				StoreDecorator(new DefaultDecorator(name, page, uriPath, role, parameters));
			}
		}


		private string GetContainedText(XmlNode parent, string childTagName)
		{
			XmlNode tag = ((XmlElement) parent).GetElementsByTagName(childTagName).Item(0);
			string text = ((XmlText) tag.FirstChild).Data;
			return text;
		}

		private string GetAttribute(XmlElement element, string name)
		{
			if (element != null && element.GetAttribute(name) != null && element.GetAttribute(name).Trim() != string.Empty)
			{
				return element.GetAttribute(name).Trim();
			}
			else
			{
				return null;
			}
		}

		private void StoreDecorator(IDecorator d)
		{
			if (d.Role != null)
			{
				decorators.Add(d.Name + d.Role, d);
			}
			else
			{
				decorators.Add(d.Name, d);
			}
		}
	}
}