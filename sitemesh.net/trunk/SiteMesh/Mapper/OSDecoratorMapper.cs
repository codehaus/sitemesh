using System;
using System.Collections;
using System.Web;

namespace SiteMesh.Mapper
{
	/// <summary>
	/// The OSDecoratorMapper will map a suitable decorator based on the operating system
	/// of the remote client.
	///
	/// <p>OSDecoratorMapper works by checking to see if the "UA-OS" header
	/// was sent with the HTTP request. If it was, the class will check the
	/// value of the header with all the different os's the user has configured
	/// the Decorator Mapper to identify and, if a match is found, routes the
	/// request accordingly.  Configuration is done using the sitemesh.xml file.
	/// The param name is a string literal (operating system name) you would like
	/// to match in the UA-OS header, and the value is what will be appended to the
	/// decorator name if the user is using that operating system</p>
	///
	/// @author	<a href="mailto:schepdawg@yahoo.com">Adam P. Schepis</a>
	/// @version	$Revision: 1.2 $
	/// </summary>
public class OSDecoratorMapper : AbstractDecoratorMapper 
{
	/// <summary>
	/// Properties holds the parameters that the object was initialized with.
	/// </summary>
	protected IDictionary properties;

	/// <summary>
	/// Init initializes the OSDecoratorMapper object by setting the parent
	/// DecoratorMapper, and loading the initialization properties.
	/// </summary>
	/// <param name="properties">An object containing intialization parameters</param>
	/// <param name="parent">The parent DecoratorMapper object</param>
	public override void Init(IDictionary properties, IDecoratorMapper parent)
	{
		this.properties = properties;
		this.parent = parent;
	}

	/// <summary>
	/// Attempts to find the correct decorator for Page page based on
	/// the UA-OS HTTP header in the request.
	/// </summary>
	/// <param name="request">The HTTP request sent to the server</param>
	/// <param name="page">The page SiteMesh is trying to find a decorator for</param>
	/// <returns>
	///		A Decorator object that is either the decorator for the identified
	///		OS, or the parent DecoratorMapper's decorator
	///	</returns>
	public IDecorator HetDecorator(HttpRequest request, IPage page) 
	{
		string osHeader = request.Headers["UA-OS"];
		if (osHeader == null)
		{
			return parent.GetDecorator(request, page);
		}

		osHeader = osHeader.ToLower();

		// run through the list of operating systems the application developer listed
		// in web.config to see if we have a match to the user's current OS
		foreach (string os in properties.Keys) 
		{
			// see if the name matches the user's operating system name
			if (osHeader.IndexOf(os.ToLower()) != -1) 
			{
				string decoratorName = parent.GetDecorator(request, page).Name;
				if (decoratorName != null) 
				{
					decoratorName += '-' + properties[os].ToString();
				}
				return GetNamedDecorator(request, decoratorName);
			}
		}

		return parent.GetDecorator(request, page);
	}
}
}