using System;
using System.Collections;

namespace SiteMesh.Factory
{
	/// <summary>
	/// Base Factory implementation. Provides utility methods for implementation.
	/// 
	/// @author <a href="mailto:joe@truemesh.com">Joe Walnes</a>
	/// @version $Revision: $
	/// </summary>
	public abstract class BaseFactory : ConfigFactory
	{

		/// <summary>
		/// Instance of {@link com.opensymphony.module.sitemesh.DecoratorMapper}.
		/// Because it is thread-safe, it can be shared by multiple clients. This
		/// is only the last DecoratorMapper in the chain, and all parents will be
		/// automatically delegated to it.
		/// </summary>
		protected IDecoratorMapper decoratorMapper = null;

		/// <summary>
		/// Map that associates content-types with PageParser instances. 
		/// </summary>
		protected IDictionary pageParsers = null;

		/// <summary>
		/// Constructor for default implementation of Factory.
		/// Should never be called by client. Singleton instance should be
		/// obtained instead.
		/// </summary>
		protected BaseFactory()
		{
			ClearDecoratorMappers();
			ClearParserMappings();
		}

		/// <summary>
		/// Return instance of DecoratorMapper.
		/// </summary>
		/// <returns></returns>
		public override IDecoratorMapper GetDecoratorMapper()
		{
			return decoratorMapper;
		}

		/// <summary>
		/// Create a PageParser suitable for the given content-type.
		/// <p>For example, if the supplied parameter is <code>text/html</code>
		/// a parser shall be returned that can parse HTML accordingly. Returns
		/// null if no parser can be found for the supplied content type.</p>
		/// </summary>
		/// <param name="contentType">
		///		The MIME content-type of the data to be parsed
		///	</param>
		/// <returns>
		///		Appropriate <code>PageParser</code> for reading data, or
		///		<code>null</code> if no suitable parser was found.
		/// </returns>
		public override IPageParser GetPageParser(String contentType)
		{
			return (IPageParser) pageParsers[contentType];
		}

		/// <summary>
		/// Determine whether a Page of given content-type should be parsed or not.
		/// </summary>
		/// <param name="contentType">Content type of page</param>
		/// <returns></returns>
		public override bool ShouldParsePage(String contentType)
		{
			return pageParsers.Contains(contentType);
		}

		/// <summary>
		/// Clear all current DecoratorMappers.
		/// </summary>
		protected void ClearDecoratorMappers()
		{
			decoratorMapper = null;
		}

		/// <summary>
		/// Push new DecoratorMapper onto end of chain.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="properties"></param>
		protected void PushDecoratorMapper(String typeName, IDictionary properties)
		{
			try
			{
				Type decoratorType = Type.GetType(typeName);
				IDecoratorMapper newMapper = (IDecoratorMapper) Activator.CreateInstance(decoratorType);
				newMapper.Init(properties, decoratorMapper);
				decoratorMapper = newMapper;

				// newMapper.init(config, properties, decoratorMapper);
			}
			catch (Exception e)
			{
				Report("Could not initialize DecoratorMapper : " + typeName, e);
			}
		}

		/// <summary>
		/// Clear all PageParser mappings.
		/// </summary>
		protected void ClearParserMappings()
		{
			pageParsers = new Hashtable();
		}



		/// <summary>
		///	Map new PageParser to given content-type. contentType = null signifies default
		/// PageParser for unknown content-types.
		/// </summary>
		/// <param name="contentType">Content type parser handles</param>
		/// <param name="typeName">PageParser type to use</param>
		protected void MapParser(string contentType, string typeName)
		{
			try
			{
				Type parserType = Type.GetType(typeName, true, false);
				IPageParser pp = (IPageParser) Activator.CreateInstance(parserType);

				// Store the parser even if the content type is NULL. [This
				// is most probably the legacy default page parser which
				// we no longer have a use for]
				pageParsers.Add(contentType, pp);
			}

			catch (Exception e)
			{
				Report("Could not instantiate PageParser : " + typeName, e);
			}
		}
	}
}