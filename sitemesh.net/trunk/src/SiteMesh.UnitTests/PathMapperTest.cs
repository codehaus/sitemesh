using NUnit.Framework;

namespace SiteMesh.Test.UnitTest
{
	[TestFixture]
	public class PathMapperTest
	{
		public PathMapperTest()
		{
		}

		private PathMapper pm;

		[SetUp]
		public void Init()
		{
			pm = new PathMapper();

			// exact matches come first
			pm.Add("exact1", "/myexactfile.html");
			pm.Add("exact2", "/mydir/myexactfile.html");
			pm.Add("exact3", "/mydir/myexactfile.jsp");
			pm.Add("exact4", "/mydir/dodo");

			// then the complex matches
			pm.Add("complex1", "/mydir/*");
			pm.Add("complex2", "/mydir/otherdir/*.jsp");
			pm.Add("complex3", "/otherdir/*.??p");
			pm.Add("complex4", "*.xml");
			pm.Add("complex5", "/*/admin/*.??ml");
			pm.Add("complex6", "/*/complexx/a*b.x?tml");

			// if all the rest fails, use the default matches
			pm.Add("default", "*");
		}

		[Test]
		public void TestHardening()
		{
			PathMapper bad = new PathMapper();
			bad.Add(null, null);
			Assert.AreEqual(null, bad.Item(null));
			Assert.AreEqual(null, bad.Item(""));
			Assert.AreEqual(null, bad.Item("/somenonexistingpath"));
		}

		[Test]
		public void TestExactKey()
		{
			Assert.AreEqual("exact1", pm.Item("/myexactfile.html"));
			Assert.AreEqual("exact2", pm.Item("/mydir/myexactfile.html"));
			Assert.AreEqual("exact3", pm.Item("/mydir/myexactfile.jsp"));
			Assert.AreEqual("exact4", pm.Item("/mydir/dodo"));
		}

		[Test]
		public void TestComplexKey()
		{
			Assert.AreEqual("complex1", pm.Item("/mydir/"));
			Assert.AreEqual("complex1", pm.Item("/mydir/test1.xml"));
			Assert.AreEqual("complex1", pm.Item("/mydir/test321.jsp"));
			Assert.AreEqual("complex1", pm.Item("/mydir/otherdir"));

			Assert.AreEqual("complex2", pm.Item("/mydir/otherdir/test321.jsp"));

			Assert.AreEqual("complex3", pm.Item("/otherdir/test2.jsp"));
			Assert.AreEqual("complex3", pm.Item("/otherdir/test2.bpp"));

			Assert.AreEqual("complex4", pm.Item("/somedir/one/two/some/deep/file/test.xml"));
			Assert.AreEqual("complex4", pm.Item("/somedir/321.jsp.xml"));

			Assert.AreEqual("complex5", pm.Item("/mydir/otherdir/admin/myfile.html"));
			Assert.AreEqual("complex5", pm.Item("/mydir/somedir/admin/text.html"));

			Assert.AreEqual("complex6", pm.Item("/mydir/complexx/a-some-test-b.xctml"));
			Assert.AreEqual("complex6", pm.Item("/mydir/complexx/a b.xhtml"));
			Assert.AreEqual("complex6", pm.Item("/mydir/complexx/a___b.xhtml"));
		}

		[Test]
		public void TestFindDefaultKey()
		{
			Assert.AreEqual("default", pm.Item(null));
			Assert.AreEqual("default", pm.Item("/"));
			Assert.AreEqual("default", pm.Item("/*"));
			Assert.AreEqual("default", pm.Item("*"));
			Assert.AreEqual("default", pm.Item("blah.txt"));
			Assert.AreEqual("default", pm.Item("somefilewithoutextension"));
			Assert.AreEqual("default", pm.Item("/file_with_underscores-and-dashes.test"));
			Assert.AreEqual("default", pm.Item("/tuuuu*/file.with.dots.test.txt"));
		}
	}
}