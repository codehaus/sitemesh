using System.Collections;
using System.Web;

namespace SiteMesh.Mapper
{
	/// <summary>
	/// Abstract DecoratorMapper implementation for easy creation of new DecoratorMappers.
	///
	/// <p>Typically, an implementation would override getNamedDecorator() <b>or</b> getDecorator().
	/// If a Decorator cannot be returned from either of these, then they should delegate to their
	/// superclass.</p>
	///
	/// @author <a href="mailto:joe@truemesh.com">Joe Walnes</a>
	/// @version $Revision: 1.1 $
	///
	/// @see com.opensymphony.module.sitemesh.DecoratorMapper
	/// </summary>
	public abstract class AbstractDecoratorMapper : IDecoratorMapper
	{
		/// <summary>
		/// Parent DecoratorMapper.
		/// </summary>
		protected IDecoratorMapper parent = null;

		/// <summary>
		/// Sets parent.
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="parent"></param>
		public virtual void Init(IDictionary properties, IDecoratorMapper parent)
		{
			this.parent = parent;
		}

		/// <summary>
		/// Delegates to parent.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		public virtual IDecorator GetDecorator(HttpRequest request, IPage page)
		{
			return parent.GetDecorator(request, page);
		}

		/// <summary>
		///  Delegate to parent.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public virtual IDecorator GetNamedDecorator(HttpRequest request, string name)
		{
			return parent.GetNamedDecorator(request, name);
		}
	}
}