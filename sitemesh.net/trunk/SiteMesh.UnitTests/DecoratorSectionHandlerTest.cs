using System.Text;
using System.Xml;
using NUnit.Framework;
using SiteMesh.Mapper;

namespace SiteMesh.Test.UnitTest
{
	[TestFixture]
	public class DecoratorSectionHandlerTest
	{
		private DecoratorSectionHandler handler;

		[SetUp]
		public void Init()
		{
			handler = new DecoratorSectionHandler();
			XmlDocument decoratorNode = new XmlDocument();
			StringBuilder decorators = new StringBuilder();

			decorators.Append("<decorators defaultDir=\"/decorators\">");
			decorators.Append("   <decorator name=\"default\" page=\"default.jsp\">");
			decorators.Append("       <pattern>/info/*p?ge*l</pattern>");
			decorators.Append("       <pattern>/test/*</pattern>");
			decorators.Append("   </decorator>");

			decorators.Append("   <decorator name=\"other\" page=\"/other.jsp\">");
			decorators.Append("       <pattern>/other/*</pattern>");
			decorators.Append("   </decorator>");

			decorators.Append("   <decorator name=\"uri\" page=\"uri.jsp\" webapp=\"someapp\">");
			decorators.Append("       <pattern>/uri/*</pattern>");
			decorators.Append("   </decorator>");

			decorators.Append("   <decorator name=\"rolebased\" page=\"/rolebased.jsp\" role=\"developer\">");
			decorators.Append("       <pattern>/rolebased/*</pattern>");
			decorators.Append("   </decorator>");

			decorators.Append("</decorators>");
			decoratorNode.LoadXml(decorators.ToString());
			handler.Create(null, null, decoratorNode.DocumentElement);
		}

		[Test]
		public void TestMappedNames()
		{
			Assert.AreEqual("default", handler.GetMappedName("/info/somepage.html"));
			Assert.AreEqual("default", handler.GetMappedName("/test/somepage.html"));
			Assert.AreEqual("other", handler.GetMappedName("/other/someotherpage.html"));
			Assert.AreEqual("uri", handler.GetMappedName("/uri/somepage.html"));
			Assert.AreEqual("rolebaseddeveloper", handler.GetMappedName("/rolebased/someotherpage.html"));

		}

		[Test]
		public void TestDecoratorPresence()
		{
			Assert.IsNotNull(handler.GetDecoratorByName("default"));
			Assert.IsNotNull(handler.GetDecoratorByName("other"));
			Assert.IsNotNull(handler.GetDecoratorByName("uri"));
			Assert.IsNotNull(handler.GetDecoratorByName("rolebaseddeveloper"));
		}

		[Test]
		public void TestDecorators()
		{
			Assert.AreEqual(handler.GetDecoratorByName("default").Name, "default");
			Assert.AreEqual(handler.GetDecoratorByName("default").Page, "/decorators/default.jsp");
			Assert.IsNull(handler.GetDecoratorByName("default").UriPath);
			Assert.IsNull(handler.GetDecoratorByName("default").Role);

			Assert.AreEqual(handler.GetDecoratorByName("other").Name, "other");
			Assert.AreEqual(handler.GetDecoratorByName("other").Page, "/decorators/other.jsp");
			Assert.IsNull(handler.GetDecoratorByName("other").UriPath);
			Assert.IsNull(handler.GetDecoratorByName("other").Role);

			Assert.AreEqual(handler.GetDecoratorByName("uri").Name, "uri");
			Assert.AreEqual(handler.GetDecoratorByName("uri").Page, "/decorators/uri.jsp");
			Assert.AreEqual(handler.GetDecoratorByName("uri").UriPath, "/someapp");
			Assert.IsNull(handler.GetDecoratorByName("uri").Role);

			Assert.AreEqual(handler.GetDecoratorByName("rolebaseddeveloper").Name, "rolebased");
			Assert.AreEqual(handler.GetDecoratorByName("rolebaseddeveloper").Page, "/decorators/rolebased.jsp");
			Assert.IsNull(handler.GetDecoratorByName("rolebaseddeveloper").UriPath);
			Assert.AreEqual(handler.GetDecoratorByName("rolebaseddeveloper").Role, "developer");
		}
	}
}