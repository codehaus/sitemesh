using System.Web.UI;

namespace SiteMesh
{
	/// <summary>
	/// Summary description for BaseDecoratorPage.
	/// </summary>
	public class BaseDecoratorPage : Page
	{
		public BaseDecoratorPage()
		{
		}

		protected override object LoadPageStateFromPersistenceMedium()
		{
			return null;
		}
	}
}