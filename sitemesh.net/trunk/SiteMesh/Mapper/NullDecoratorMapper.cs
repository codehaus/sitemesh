using System.Collections;
using System.Web;

namespace SiteMesh.Mapper
{
	/// <summary>
	/// The NullDecoratorMapper represents the top-level DecoratorMapper that
	/// is finally delegated to if no other DecoratorMapper has intervened.
	/// It is used so the parent property does not have to be checked by
	/// other DecoratorMappers (null object pattern).
	///
	/// @author <a href="mailto:joe@truemesh.com">Joe Walnes</a>
	/// </summary>
	public class NullDecoratorMapper : IDecoratorMapper
	{
		/// <summary>
		/// Does nothing.
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="parent"></param>
		public void Init(IDictionary properties, IDecoratorMapper parent)
		{
		}

		/// <summary>
		/// Returns null.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		public IDecorator GetDecorator(HttpRequest request, IPage page)
		{
			return null;
		}

		/// <summary>
		/// Returns null.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public IDecorator GetNamedDecorator(HttpRequest request, string name)
		{
			return null;
		}
	}
}