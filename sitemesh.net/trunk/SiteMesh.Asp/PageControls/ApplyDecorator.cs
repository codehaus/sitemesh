using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.UI;

namespace SiteMesh.Asp.PageControls
{
	/// <summary>
	/// Summary description for ApplyDecorator.
	/// </summary>
	[ToolboxData("<{0}:ApplyDecorator runat=server></{0}:ApplyDecorator>"), ParseChildren(false)]
	public class ApplyDecorator : Control, INamingContainer
	{
		private string decorator;
		private string url;

		public ApplyDecorator()
		{
		}

		public string Url
		{
			get { return url; }
			set { url = value; }
		}

		public string Decorator
		{
			get { return decorator; }
			set { decorator = value; }
		}

		protected override void Render(HtmlTextWriter output)
		{
			IPage parsedPage = ParsePage();

			if (parsedPage != null)
			{
				// AddParameters(parsedPage)
				IDecorator d = GetDecorator(parsedPage);
				ApplyDecoratorToPage(d, parsedPage);
			}
		}

		private IDecorator GetDecorator(IPage parsedPage)
		{
			IDecorator d;
			Context.Items[SiteMeshConstants.DECORATOR] = Decorator;
			d = ConfigFactory.GetInstance().GetDecoratorMapper().GetDecorator(Context.Request, parsedPage);
			Context.Items.Remove(SiteMeshConstants.DECORATOR);
			return d;
		}

		private void ApplyDecoratorToPage(IDecorator d, IPage parsedPage)
		{
			if (d != null && d.Page != null)
			{
				Context.Items.Add(SiteMeshConstants.PAGE, parsedPage);
				Page.Server.Execute(d.Page);
				Context.Items.Remove(SiteMeshConstants.PAGE);
			}
		}

		private IPage ParsePage()
		{
			IPageParser parser;
			IPage parsedPage;
			byte[] buffer;

			if (url == null)
			{
				buffer = GetInlineContent();
			}
			else
			{
				// GetExternalContent
				// currently not supported
				throw new Exception("external content currently not supported");
			}

			// parse the page
			parser = ConfigFactory.GetInstance().GetPageParser("text/html");
			parsedPage = parser.Parse(buffer);

			return parsedPage;
		}

		private byte[] GetInlineContent()
		{
			byte[] buffer;
			StringWriter strWriter = new StringWriter(CultureInfo.CurrentUICulture);
			HtmlTextWriter tempWriter = new HtmlTextWriter(strWriter);

			RenderChildren(tempWriter);
			// hard coded ascii encoding, probably needs to be based upon content type

			Encoding ascii = new ASCIIEncoding();
			buffer = ascii.GetBytes(strWriter.GetStringBuilder().ToString().ToCharArray());

			return buffer;
		}

	}
}