using System;
using System.Web.UI;

namespace SiteMesh.Asp.DecoratorControls
{
	/// <summary>
	/// Writes the origial head section of requested page.
	/// </summary>
	[ToolboxData("<{0}:Head runat=server></{0}:Head>")]
	public class Head : System.Web.UI.Control
	{
		protected override void Render(HtmlTextWriter output)
		{
			IPage p = (IPage) Context.Items[SiteMeshConstants.PAGE];
			output.Write(p.Head);
		}

	}
}