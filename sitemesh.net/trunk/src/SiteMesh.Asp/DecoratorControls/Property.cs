using System.Web.UI;

namespace SiteMesh.Asp.DecoratorControls
{
	/// <summary>
	/// Write property of Page to out.
	/// </summary>
	[ToolboxData("<{0}:GetProperty runat=server></{0}:GetProperty>")]
	public class GetProperty : Control
	{
		private string name;

		/// <summary>
		/// Name of the property to include.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		protected override void Render(HtmlTextWriter output)
		{
			IPage p = (IPage) Context.Items[SiteMeshConstants.PAGE];

			if (p != null && name != null)
			{
				output.WriteLine(p.Properties[name]);
			}
		}
	}
}