using System.IO;
using System.Web;

namespace SiteMesh.Asp
{
	/// <summary>
	/// The default System.Web.StaticFileHandler uses page caching when
	/// processing static file contents such as html files. SiteMesh
	/// needs the page to be written to the response stream for each request
	/// in order to decorate the page. 
	/// This handler serves the page without caching enabling decoration.
	/// Thanks to Darren Syzling for providing this class.
	/// </summary>
	public class HtmlFileHandler : IHttpHandler
	{
		public bool IsReusable
		{
			get { return true; }
		}

		public void ProcessRequest(HttpContext context)
		{
			string path = context.Request.PhysicalPath;

			if (!File.Exists(path))
			{
				throw new HttpException(404, "File does not exist");
			}

			context.Response.WriteFile(path);
		}
	}
}