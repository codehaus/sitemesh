namespace SiteMesh
{
	/// <summary>
	/// A set of static constants of Strings to be used as ServletRequest attribute keys
	/// to represent various objects passed between pages.
	/// 
	/// @author <a href="joe@truemesh.com">Joe Walnes</a>
	/// </summary>
	public sealed class SiteMeshConstants
	{
		/// <summary>
		/// Stores SiteMesh.IPage instance for parsed page to be
		/// passed across to SiteMesh.IDecorator.
		/// </summary>
		public static readonly string PAGE = "__sitemesh__page";

		/// <summary>
		/// The name (String) of the Decorator to suggest using. This is set by
		/// the SiteMesh.PageControls.ApplyDecoratorTag
		/// and used by the corresponding SiteMesh.DecoratorMapper.
		/// </summary>
		public static readonly string DECORATOR = "__sitemesh__decorator";
	}
}