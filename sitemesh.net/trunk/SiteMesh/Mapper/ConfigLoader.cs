using System;

namespace SiteMesh.Mapper
{
	/// <summary>
	/// The ConfigLoader reads a configuration XML file that contains Decorator definitions
	/// (name, url, init-params) and path-mappings (pattern, name).
	///
	/// <p>These can then be accessed by the getDecoratorByName() methods and getMappedName()
	/// methods respectively.</p>
	///
	/// <p>The DTD for the configuration file in old (deprecated) format is located at
	/// <a href="http://www.opensymphony.com/dtds/sitemesh_1_0_decorators.dtd">
	///  http://www.opensymphony.com/dtds/sitemesh_1_0_decorators.dtd
	/// </a>.</p>
	///
	/// <p>The DTD for the configuration file in new format is located at
	/// <a href="http://www.opensymphony.com/dtds/sitemesh_1_5_decorators.dtd">
	///  http://www.opensymphony.com/dtds/sitemesh_1_5_decorators.dtd
	/// </a>.</p>
	///
	/// <p>Editing the config file will cause it to be auto-reloaded.</p>
	///
	/// <p>This class is used by ConfigDecoratorMapper, and uses PathMapper for pattern matching.</p>
	///
	/// @author <a href="mailto:joe@truemesh.com">Joe Walnes</a>
	/// @author <a href="mailto:pathos@pandora.be">Mathias Bogaert</a>
	/// @version $Revision: 1.3 $
	/// </summary>
	public sealed class ConfigLoader
	{
		/// <summary>
		/// Create new ConfigLoader using supplied filename and config.
		/// </summary>
		/// <param name="configFileName"></param>
		public ConfigLoader(String configFileName)
		{
		}

	}
}