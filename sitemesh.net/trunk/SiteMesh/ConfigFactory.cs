using System;
using System.Configuration;
using SiteMesh.Factory;

namespace SiteMesh
{
	public abstract class ConfigFactory
	{
		protected static ConfigFactory instance;

		protected ConfigFactory()
		{
		}

		public static ConfigFactory GetInstance()
		{
			if (instance == null)
			{
				string factoryClass = GetAppSetting("sitemesh.factory", "SiteMesh.Factory.DefaultFactory, SiteMesh");
				try
				{
					Type factoryType = Type.GetType(factoryClass, true, false);
					instance = (ConfigFactory) Activator.CreateInstance(factoryType);
				}
				catch (Exception e)
				{
					throw new ApplicationException("Unable to load config factory", e);
				}
			}
			return instance;
		}

		public abstract IPageParser GetPageParser(string contentType);

		public abstract IDecoratorMapper GetDecoratorMapper();

		public abstract bool ShouldParsePage(string contentType);

		public static string GetAppSetting(string key, string defaultValue)
		{
			string setting = ConfigurationSettings.AppSettings[key];
			if (setting == null || setting == string.Empty)
			{
				setting = defaultValue;
			}
			return setting;
		}

		/// <summary>
		/// Report a problem.
		/// </summary>
		/// <param name="msg">Message describing the problem.</param>
		/// <param name="e">Possible exception</param>
		protected static void Report(String msg, Exception e) 
		{
			throw new FactoryException(msg, e);
		}

	}
}