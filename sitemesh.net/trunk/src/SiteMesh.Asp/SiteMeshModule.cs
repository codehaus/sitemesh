using System;
using System.Collections;
using System.Web;

namespace SiteMesh.Asp
{
	/// <summary>
	/// ASP.NET module to catch original request.
	/// </summary>
	public class SiteMeshModule : IHttpModule
	{

		private const string FILTER_KEY = "__sitemesh_filter";

		public void Init(HttpApplication context)
		{
			context.BeginRequest += new EventHandler(this.BeginRequestHandler);
			context.EndRequest += new EventHandler(this.EndRequestHandler);
		}

		public void Dispose()
		{
		}

		// What the heck???!

		//Public Sub PreSendRequestHeadersHandler(ByVal s As Object, ByVal e As EventArgs)
		//   Dim app As HttpApplication = CType(s, HttpApplication)
		//    If app.Response.StatusCode = 304 Then
		//        'app.Response.StatusCode = 200
		//        'app.Response.StatusDescription = "OK"
		//    End If
		//End Sub

		public void BeginRequestHandler(object o, EventArgs e)
		{
			ResponseFilter pageFilter = null;
			HttpApplication app = (HttpApplication) o;
			pageFilter = new ResponseFilter(app);
			app.Context.Items.Add(FILTER_KEY, pageFilter);
			app.Response.Filter = pageFilter;
		}

		public void EndRequestHandler(object o, EventArgs e)
		{
			HttpApplication app = (HttpApplication) o;
			ResponseFilter pageFilter;
			ConfigFactory factory;
			pageFilter = (ResponseFilter) app.Context.Items[FILTER_KEY];

			// parse the page
			factory = ConfigFactory.GetInstance();
			IPageParser parser = factory.GetPageParser("text/html");
			IPage parsedPage = parser.Parse(pageFilter.Buffer);

			if (parsedPage != null)
			{
				// look for a decorator
				IDecorator d = factory.GetDecoratorMapper().GetDecorator(app.Context.Request, parsedPage);
				if (d != null && d.Page != null)
				{
					app.Context.Items.Add(SiteMeshConstants.PAGE, parsedPage);
					app.Server.Execute(d.Page);
					IEnumerator paramNames = d.GetInitParameterNames();

					while (paramNames.MoveNext())
					{
						string initParam = (string) paramNames.Current;
						if (initParam.StartsWith("header."))
						{
							app.Response.AppendHeader(initParam.Substring(initParam.IndexOf(".")), d.GetInitParameter(initParam));
						}
					}

					app.Context.Items.Remove(SiteMeshConstants.PAGE);
					parsedPage = null;
					return;
				}

				// if none found, use original stream
				parsedPage.WritePage(app.Response.OutputStream);
				parsedPage = null;
			}

		}
	}
}