using System.Web.UI;

namespace SiteMesh.Asp.DecoratorControls
{
	/// <summary>
	///  Returns the title of requested page.
	/// </summary>
	[ToolboxData("<{0}:Title runat=server></{0}:Title>")]
	public class Title : Control
	{
		/// <summary>
		/// Default title.
		/// </summary>
		private string defaultTitle;

		/// <summary>
		/// The title to show if none can be found from requested page.
		/// </summary>
		public string DefaultTitle
		{
			get { return defaultTitle; }
			set { defaultTitle = value; }
		}

		protected override void Render(HtmlTextWriter output)
		{
			IPage p = (IPage) Context.Items[SiteMeshConstants.PAGE];
			if (p.Title == null || (p.Title == string.Empty))
			{
				output.Write(DefaultTitle);
			}
			else
			{
				output.Write(p.Title);
			}
		}

	}
}