using System;
using System.Collections;
using System.IO;
using System.Web;

namespace SiteMesh.Parser
{
	/// <summary>
	/// Abstract implementation of {@link com.opensymphony.module.sitemesh.Page} .
	///
	/// <p>Contains base methods for storing and accessing page properties.
	/// Also stores pageData as byte[] and implements write???() methods.</p>
	///
	/// <p>Concrete implementations need only set the pageData and call 
	/// addProperty(string, string)} to add all the required information.</p>
	///
	/// @author <a href="joe@truemesh.com">Joe Walnes</a>
	/// </summary>
	public abstract class AbstractPage : IPage
	{
		/// <summary>
		/// Map of all properties.
		/// Key is String. Value is java.util.List of multiple String values.
		/// </summary>
		private IDictionary properties = new Hashtable();

		/// <summary>
		/// Data of page contents.
		/// </summary>
		protected byte[] pageData = new byte[0];

		/// <summary>
		/// RequestURI of original Page.
		/// </summary>
		private HttpRequest request;

		public virtual void WritePage(Stream outWriter)
		{
			outWriter.Write(pageData, 0, pageData.Length);
		}

		public virtual string Page
		{
			get
			{
				return new SiteMesh.Parser.Util.CharArray(1024).append(pageData).ToString();
			}
		}

		/// <summary>
		/// Write data of html <code>&lt;body&gt;</code> tag.
		///
		/// <p>Must be implemented. Data written should not actually contain the
		/// body tags, but all the data in between.</p>
		/// </summary>
		/// <param name="outWriter">Writer to write to.</param>
		public abstract void WriteBody(TextWriter outWriter);

		/// <summary>
		/// Return title of from "title" property. Never returns null. 
		/// </summary>
		public virtual string Title
		{
			get { return noNull(GetProperty("title")); }
		}

		/// <summary>
		/// Return head section of page. Never returns null. 
		/// </summary>
		public virtual string Head
		{
			get { return noNull(GetProperty("head")); }
		}

		/// <summary>
		/// Return body section of page. Never returns null. 
		/// </summary>
		public virtual string Body
		{
			get { return noNull(GetProperty("body")); }
		}


		public virtual int ContentLength
		{
			get { return pageData.Length; }
		}

		public String GetProperty(String name)
		{
			if (!PropertySet(name))
				return null;
			return (string) properties[name];
		}

		public int GetIntProperty(String name)
		{
			try
			{
				return Convert.ToInt32(noNull(GetProperty(name)));
			}
			catch (Exception)
			{
				return 0;
			}
		}

		public long GetLongProperty(String name)
		{
			try
			{
				return Convert.ToInt64(noNull(GetProperty(name)));
			}
			catch (Exception)
			{
				return 0;
			}
		}

		public bool GetBooleanProperty(string name)
		{
			string property = GetProperty(name);
			if (property == null || property.Trim().Length == 0)
				return false;
			switch (property[0])
			{
				case '1':
				case 't':
				case 'T':
				case 'y':
				case 'Y':
					return true;
				default:
					return false;
			}
		}

		public bool PropertySet(string name)
		{
			return properties.Contains(name);
		}

		public string[] PropertyKeys
		{
			get
			{
				lock (properties)
				{
					string[] retValue = new String[properties.Keys.Count];
					properties.Keys.CopyTo(retValue, 0);
					return retValue;
				}
			}
		}

		public IDictionary Properties
		{
			get { return properties; }
		}

		public HttpRequest Request
		{
			get { return request; }
			set
			{
				// Create snapshot of Request.
				request = value;

			}
		}

		/// <summary>
		/// Add a property to the properties list. 
		/// </summary>
		/// <param name="name">Name of property</param>
		/// <param name="data">Value of property</param>
		public void AddProperty(string name, string data)
		{
			properties.Add(name, data);
		}

		/// <summary>
		/// Return String as is, or "" if null. (Prevents NullReferenceExceptions)
		/// </summary>
		/// <param name="text">Text to check</param>
		/// <returns>empty if null, otherwise text</returns>
		protected static string noNull(String text)
		{
			return text == null ? string.Empty : text;
		}
	}

}