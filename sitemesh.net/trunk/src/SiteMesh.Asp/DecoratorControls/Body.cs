using System.Web.UI;

namespace SiteMesh.Asp.DecoratorControls
{
	/// <summary>
	/// Writes original target page body.
	/// </summary>
	[ToolboxData("<{0}:Body runat=server></{0}:Body>")]
	public class Body : System.Web.UI.Control
	{
		protected override void Render(HtmlTextWriter output)
		{
			IPage p = (IPage) Context.Items[SiteMeshConstants.PAGE];
			p.WriteBody(output);
		}
	}
}