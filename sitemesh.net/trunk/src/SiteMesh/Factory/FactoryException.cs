using System;

namespace SiteMesh.Factory
{
	/// <summary>
	/// This Exception is thrown by the Factory if it cannot initialize or perform
	/// an appropriate function.
	///
	/// @author <a href="mailto:joe@truemesh.com">Joe Walnes</a>
	/// @version $Revision: 1.1 $
	/// </summary>
	public class FactoryException : Exception
	{
		protected Exception exception = null;

		public FactoryException() : base()
		{
		}

		public FactoryException(string msg) : base(msg)
		{
		}


		public FactoryException(string msg, Exception e) : base(msg, e)
		{
		}
	}
}
