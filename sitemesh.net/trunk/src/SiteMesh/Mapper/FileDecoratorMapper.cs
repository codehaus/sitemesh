using System;
using System.IO;
using System.Net;
using System.Web;

namespace SiteMesh.Mapper
{
	/// <summary>
	/// The FileDecoratorMapper will treat the name of the decorator as a file-name to use
	/// (in the context of the web-app).
	///	@author <a href="joe@truemesh.com">Joe Walnes</a>
	/// @author <a href="mike@atlassian.com">Mike Cannon-Brookes</a>
	/// </summary>
	public class FileDecoratorMapper : AbstractDecoratorMapper 
	{
		private static bool pathNotAvailable = false;

		public override IDecorator GetNamedDecorator(HttpRequest request, string name)
		{
			if (pathNotAvailable || name == null) 
			{
				return base.GetNamedDecorator(request, name);
			}

			string resourcePath = null;

			// try to locate the resource (might be an unexpanded WAR)
			try  
			{
				// ?? resourcePath = request.MapPath(name);
			}
			catch
			{
				//e.printStackTrace();
				// return super.getNamedDecorator(req, name);
			}

			string filePath = request.MapPath(name);

			if (filePath == null && resourcePath == null) 
			{
				pathNotAvailable = true;
				return base.GetNamedDecorator(request, name);
			}
			else if (filePath != null) 
			{ 
				// do we really need this disk file check?!
				if (File.Exists(filePath)) 
				{
					// if filename exists with name of supplied decorator, return Decorator
					return new DefaultDecorator(name, name, null);
				}
				else 
				{
					// otherwise delegate to parent mapper.
					return base.GetNamedDecorator(request, name);
				}
			}
			else 
			{
				// file path is null and resource path is not null - can't check file on disk
				return new DefaultDecorator(name, name, null);
			}
		}
		
	}
}